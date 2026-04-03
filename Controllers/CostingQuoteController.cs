using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyTrack.API.Data;
using PolyTrack.API.Models;

namespace PolyTrack.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CostingQuoteController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CostingQuoteController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CostingQuote>>> GetQuotes()
    {
        return await _context.CostingQuotes.Include(q => q.Inquiry).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CostingQuote>> GetQuote(int id)
    {
        var quote = await _context.CostingQuotes.Include(q => q.Inquiry).FirstOrDefaultAsync(q => q.Id == id);
        if (quote == null) return NotFound();
        return quote;
    }

    [HttpPost]
    public async Task<ActionResult<CostingQuote>> CreateQuote(CostingQuote quote)
    {
        _context.CostingQuotes.Add(quote);

        var inquiry = await _context.SalesInquiries.FindAsync(quote.InquiryId);
        if (inquiry != null)
        {
            inquiry.Status = "QuoteSent";
            inquiry.UpdatedAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetQuote), new { id = quote.Id }, quote);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuote(int id, CostingQuote quote)
    {
        if (id != quote.Id) return BadRequest();

        _context.Entry(quote).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.CostingQuotes.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuote(int id)
    {
        var quote = await _context.CostingQuotes.FindAsync(id);
        if (quote == null) return NotFound();

        _context.CostingQuotes.Remove(quote);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
