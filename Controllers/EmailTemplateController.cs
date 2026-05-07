using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyTrack.API.Data;
using PolyTrack.API.Models;

namespace PolyTrack.API.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class EmailTemplateController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EmailTemplateController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmailTemplate>>> GetTemplates()
    {
        return await _context.EmailTemplates.ToListAsync();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTemplate(int id, EmailTemplate template)
    {
        if (id != template.Id) return BadRequest();

        _context.Entry(template).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        var existing = await _context.EmailTemplates.ToListAsync();
        _context.EmailTemplates.RemoveRange(existing);
        await _context.SaveChangesAsync();
        return await Seed();
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        if (await _context.EmailTemplates.AnyAsync()) return BadRequest("Already seeded");

        var templates = new List<EmailTemplate>
        {
            new EmailTemplate 
            { 
                Name = "OTP_Verification", 
                Subject = "Verification Code: {{OTP}} - PolyTrack ERP",
                Body = @"
                <html>
                <body style='background-color: #f8fafc; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; padding: 40px;'>
                    <div style='max-width: 550px; margin: auto; background: white; border-radius: 24px; overflow: hidden; box-shadow: 0 20px 25px -5px rgba(0,0,0,0.1);'>
                        <div style='background: linear-gradient(135deg, #1e40af 0%, #3b82f6 100%); padding: 40px; text-align: center;'>
                            <h1 style='color: white; margin: 0; font-size: 28px; font-weight: 900; letter-spacing: -0.5px;'>PolyTrack <span style='opacity: 0.7;'>ERP</span></h1>
                            <p style='color: rgba(255,255,255,0.8); margin: 8px 0 0 0; font-size: 14px; font-weight: 500;'>Security Verification</p>
                        </div>
                        <div style='padding: 40px;'>
                            <h2 style='color: #1e293b; margin: 0 0 16px 0; font-size: 20px; font-weight: 800;'>Verify your identity</h2>
                            <p style='color: #64748b; font-size: 15px; line-height: 1.6;'>Dear <strong>{{CustomerName}}</strong>,</p>
                            <p style='color: #64748b; font-size: 15px; line-height: 1.6;'>To secure your sales inquiry, please use the following unique verification code. This code is valid for 10 minutes.</p>
                            
                            <div style='background: #f1f5f9; border: 2px dashed #cbd5e1; border-radius: 16px; padding: 30px; text-align: center; margin: 30px 0;'>
                                <p style='margin: 0 0 8px 0; font-size: 11px; text-transform: uppercase; font-weight: 900; color: #94a3b8; letter-spacing: 1.5px;'>Your Secure Passcode</p>
                                <div style='font-size: 42px; font-weight: 900; letter-spacing: 12px; color: #1e3a8a; font-family: monospace;'>{{OTP}}</div>
                            </div>

                            <p style='color: #94a3b8; font-size: 13px; font-style: italic;'>If you did not request this code, please ignore this email or contact our support team immediately.</p>
                        </div>
                        <div style='background: #f8fafc; padding: 24px; text-align: center; border-top: 1px solid #f1f5f9;'>
                            <p style='margin: 0; font-size: 12px; font-weight: 700; color: #cbd5e1;'>&copy; 2026 PolyTrack ERP System. All Rights Reserved.</p>
                        </div>
                    </div>
                </body>
                </html>"
            },
            new EmailTemplate 
            { 
                Name = "Inquiry_Confirmed", 
                Subject = "Inquiry Verified Successfully - INQ-{{InquiryId}}",
                Body = @"
                <html>
                <body style='background-color: #f8fafc; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; padding: 40px;'>
                    <div style='max-width: 550px; margin: auto; background: white; border-radius: 24px; overflow: hidden; box-shadow: 0 20px 25px -5px rgba(0,0,0,0.1);'>
                        <div style='background: linear-gradient(135deg, #065f46 0%, #10b981 100%); padding: 40px; text-align: center;'>
                            <div style='background: rgba(255,255,255,0.2); width: 60px; height: 60px; border-radius: 50%; margin: 0 auto 16px; display: flex; align-items: center; justify-content: center; color: white; font-size: 30px;'>✓</div>
                            <h1 style='color: white; margin: 0; font-size: 28px; font-weight: 900; letter-spacing: -0.5px;'>Verification Successful</h1>
                        </div>
                        <div style='padding: 40px;'>
                            <p style='color: #64748b; font-size: 15px; line-height: 1.6;'>Dear <strong>{{CustomerName}}</strong>,</p>
                            <p style='color: #64748b; font-size: 15px; line-height: 1.6;'>Great news! Your sales inquiry has been successfully verified. Our dedicated team is now reviewing your requirements.</p>
                            
                            <div style='background: #ecfdf5; border-left: 4px solid #10b981; padding: 20px; border-radius: 12px; margin: 24px 0;'>
                                <p style='margin: 0; color: #065f46; font-size: 14px; font-weight: 800;'>Inquiry Reference: INQ-{{InquiryId}}</p>
                                <p style='margin: 4px 0 0 0; color: #047857; font-size: 13px;'>Our sales representative will reach out to you within 24 business hours.</p>
                            </div>

                            <p style='color: #64748b; font-size: 15px; line-height: 1.6;'>Thank you for choosing PolyTrack. We look forward to working with you.</p>
                        </div>
                        <div style='background: #f8fafc; padding: 24px; text-align: center; border-top: 1px solid #f1f5f9;'>
                            <p style='margin: 0; font-size: 12px; font-weight: 700; color: #cbd5e1;'>&copy; 2026 PolyTrack ERP System. All Rights Reserved.</p>
                        </div>
                    </div>
                </body>
                </html>"
            }
        };

        _context.EmailTemplates.AddRange(templates);
        await _context.SaveChangesAsync();
        return Ok("Seeded");
    }
}
