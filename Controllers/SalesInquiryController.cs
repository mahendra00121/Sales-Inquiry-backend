using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PolyTrack.API.Data;
using PolyTrack.API.Models;
using PolyTrack.API.Services;

namespace PolyTrack.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SalesInquiryController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public SalesInquiryController(ApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    // GET: api/SalesInquiry
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SalesInquiry>>> GetInquiries()
    {
        return await _context.SalesInquiries
            .Where(i => i.Status != "OTP_PENDING") // Hide pending OTPs from main list
            .OrderByDescending(i => i.InquiryDate)
            .ToListAsync();
    }

    // GET: api/SalesInquiry/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SalesInquiry>> GetInquiry(int id)
    {
        var inquiry = await _context.SalesInquiries.FindAsync(id);

        if (inquiry == null)
        {
            return NotFound();
        }

        return inquiry;
    }

    [HttpPost]
    public async Task<ActionResult<SalesInquiry>> CreateInquiry(SalesInquiry inquiry)
    {
        inquiry.InquiryDate = DateTime.Now;
        inquiry.UpdatedAt = DateTime.Now;
        
        // Step 1: Generate a 6-digit OTP
        string otp = new Random().Next(100000, 999999).ToString();
        
        // Step 2: Set status to OTP_PENDING and store OTP in Remarks
        inquiry.Status = "OTP_PENDING";
        inquiry.Remarks = otp; 

        _context.SalesInquiries.Add(inquiry);
        await _context.SaveChangesAsync(); // COMMIT FIRST to get the Id
        
        // Log Initial Creation
        _context.InquiryLogs.Add(new InquiryLog
        {
            InquiryId = inquiry.Id, // Now Id is valid!
            Action = "Created",
            ModifiedBy = User.Identity?.Name ?? "System",
            Timestamp = DateTime.Now,
            Details = "Inquiry created (OTP Pending)"
        });

        await _context.SaveChangesAsync(); // Commit the log

        // Step 3: Send OTP Email
        if (!string.IsNullOrEmpty(inquiry.ContactEmail))
        {
            var template = await _context.EmailTemplates.FirstOrDefaultAsync(t => t.Name == "OTP_Verification");
            var subject = template?.Subject.Replace("{{OTP}}", otp) ?? $"Verification Code: {otp}";
            var body = template?.Body
                .Replace("{{OTP}}", otp)
                .Replace("{{CustomerName}}", inquiry.CustomerName) 
                ?? $"Your OTP is {otp}";

            await _emailService.SendEmailAsync(inquiry.ContactEmail, subject, body);
        }

        return CreatedAtAction(nameof(GetInquiry), new { id = inquiry.Id }, inquiry);
    }

    // POST: api/SalesInquiry/verify-otp
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] OtpVerificationRequest request)
    {
        var inquiry = await _context.SalesInquiries.FindAsync(request.InquiryId);

        if (inquiry == null || inquiry.Status != "OTP_PENDING")
        {
            return BadRequest("Invalid inquiry or already verified.");
        }

        // Check if OTP is older than 10 minutes
        if (DateTime.Now > inquiry.UpdatedAt.AddMinutes(10))
        {
            return BadRequest("OTP Expired. Please submit a new inquiry.");
        }

        if (inquiry.Remarks == request.Otp)
        {
            // OTP Matches! 
            inquiry.Status = "New"; // Activate the inquiry
            inquiry.Remarks = "";   // Clear the OTP from remarks
            inquiry.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            // Create a notification for the system
            var notification = new Notification
            {
                Message = $"New Sales Inquiry verified from {inquiry.CustomerName}",
                Type = "Inquiry",
                RelatedId = inquiry.Id.ToString(),
                CreatedAt = DateTime.Now
            };
            _context.Notifications.Add(notification);

            // Log Verification
            _context.InquiryLogs.Add(new InquiryLog
            {
                InquiryId = inquiry.Id,
                Action = "Verified",
                ModifiedBy = "System",
                Timestamp = DateTime.Now,
                Details = "Inquiry verified via OTP"
            });

            await _context.SaveChangesAsync();

            // Send Confirmation Email using template
            if (!string.IsNullOrEmpty(inquiry.ContactEmail))
            {
                var template = await _context.EmailTemplates.FirstOrDefaultAsync(t => t.Name == "Inquiry_Confirmed");
                var subject = template?.Subject.Replace("{{InquiryId}}", inquiry.Id.ToString()) ?? "Inquiry Verified";
                var body = template?.Body
                    .Replace("{{InquiryId}}", inquiry.Id.ToString())
                    .Replace("{{CustomerName}}", inquiry.CustomerName)
                    ?? "Your inquiry has been verified.";

                await _emailService.SendEmailAsync(inquiry.ContactEmail, subject, body);
            }

            return Ok(new { message = "Inquiry verified successfully!" });
        }

        return BadRequest("Invalid verification code. Please try again.");
    }

    public class OtpVerificationRequest
    {
        public int InquiryId { get; set; }
        public string Otp { get; set; } = string.Empty;
    }

    // PUT: api/SalesInquiry/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInquiry(int id, SalesInquiry inquiry)
    {
        if (id != inquiry.Id)
        {
            return BadRequest();
        }

        inquiry.UpdatedAt = DateTime.Now;

        // Ensure we are not tracking another instance of the same ID
        var existingInquiry = await _context.SalesInquiries.FindAsync(id);
        if (existingInquiry == null) return NotFound();
        _context.Entry(existingInquiry).State = EntityState.Detached;

        _context.Entry(inquiry).State = EntityState.Modified;

        try
        {
            // Log Update
            _context.InquiryLogs.Add(new InquiryLog
            {
                InquiryId = id,
                Action = "Updated",
                ModifiedBy = User.Identity?.Name ?? "Admin",
                Timestamp = DateTime.Now,
                Details = $"Inquiry updated. Status: {inquiry.Status}"
            });

            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!InquiryExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/SalesInquiry/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInquiry(int id)
    {
        var inquiry = await _context.SalesInquiries.FindAsync(id);
        if (inquiry == null)
        {
            return NotFound();
        }

        _context.SalesInquiries.Remove(inquiry);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool InquiryExists(int id)
    {
        return _context.SalesInquiries.Any(e => e.Id == id);
    }

    // GET: api/SalesInquiry/5/logs
    [HttpGet("{id}/logs")]
    public async Task<ActionResult<IEnumerable<InquiryLog>>> GetInquiryLogs(int id)
    {
        return await _context.InquiryLogs
            .Where(l => l.InquiryId == id)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync();
    }
}
