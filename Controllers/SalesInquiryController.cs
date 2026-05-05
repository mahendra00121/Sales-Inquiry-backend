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
        return await _context.SalesInquiries.OrderByDescending(i => i.InquiryDate).ToListAsync();
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

    // POST: api/SalesInquiry
    [HttpPost]
    public async Task<ActionResult<SalesInquiry>> CreateInquiry(SalesInquiry inquiry)
    {
        inquiry.InquiryDate = DateTime.Now;
        inquiry.UpdatedAt = DateTime.Now;
        inquiry.Status = "New";

        _context.SalesInquiries.Add(inquiry);
        await _context.SaveChangesAsync();

        // Send Confirmation Email
        if (!string.IsNullOrEmpty(inquiry.ContactEmail))
        {
            var subject = "Sales Inquiry Received - PolyTrack ERP";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #2563eb;'>Thank you for your Inquiry!</h2>
                    <p>Dear {inquiry.CustomerName},</p>
                    <p>We have successfully received your sales inquiry. Our team will review the details and get back to you shortly.</p>
                    <div style='background-color: #f8fafc; padding: 20px; border-radius: 10px; border: 1px solid #e2e8f0;'>
                        <p><strong>Inquiry ID:</strong> #{inquiry.Id}</p>
                        <p><strong>Date:</strong> {inquiry.InquiryDate:f}</p>
                        <p><strong>Requirement:</strong> {inquiry.Description}</p>
                    </div>
                    <p>Best Regards,<br/><strong>PolyTrack Sales Team</strong></p>
                </body>
                </html>";

            await _emailService.SendEmailAsync(inquiry.ContactEmail, subject, body);
        }

        return CreatedAtAction(nameof(GetInquiry), new { id = inquiry.Id }, inquiry);
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
