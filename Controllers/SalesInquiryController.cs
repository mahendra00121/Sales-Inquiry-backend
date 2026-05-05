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
        await _context.SaveChangesAsync();

        // Step 3: Send OTP Email
        if (!string.IsNullOrEmpty(inquiry.ContactEmail))
        {
            var subject = "Verification Code: " + otp + " - PolyTrack ERP";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; text-align: center;'>
                    <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #e2e8f0; border-radius: 15px;'>
                        <h2 style='color: #2563eb;'>Verify Your Inquiry</h2>
                        <p>Dear {inquiry.CustomerName},</p>
                        <p>Thank you for reaching out to us. Please use the verification code below to complete your inquiry submission:</p>
                        <div style='background-color: #f1f5f9; padding: 15px; font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #1e293b; margin: 20px 0; border-radius: 10px;'>
                            {otp}
                        </div>
                        <p style='color: #64748b; font-size: 12px;'>This code will expire shortly. If you did not request this, please ignore this email.</p>
                        <hr style='border: 0; border-top: 1px solid #e2e8f0; margin: 20px 0;' />
                        <p>Best Regards,<br/><strong>PolyTrack ERP System</strong></p>
                    </div>
                </body>
                </html>";

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

        if (inquiry.Remarks == request.Otp)
        {
            // OTP Matches! 
            inquiry.Status = "New"; // Activate the inquiry
            inquiry.Remarks = "";   // Clear the OTP from remarks
            inquiry.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
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
        _context.Entry(inquiry).State = EntityState.Modified;

        try
        {
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
}
