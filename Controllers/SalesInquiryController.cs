using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyTrack.API.Data;
using PolyTrack.API.Models;

namespace PolyTrack.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SalesInquiryController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SalesInquiryController(ApplicationDbContext context)
    {
        _context = context;
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
