using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyTrack.API.Data;
using PolyTrack.API.Models;

namespace PolyTrack.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FeasibilityReviewController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FeasibilityReviewController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FeasibilityReview>>> GetReviews()
    {
        return await _context.FeasibilityReviews.Include(r => r.Inquiry).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FeasibilityReview>> GetReview(int id)
    {
        var review = await _context.FeasibilityReviews.Include(r => r.Inquiry).FirstOrDefaultAsync(r => r.Id == id);
        if (review == null) return NotFound();
        return review;
    }

    [HttpPost]
    public async Task<ActionResult<FeasibilityReview>> CreateReview(FeasibilityReview review)
    {
        _context.FeasibilityReviews.Add(review);
        
        var inquiry = await _context.SalesInquiries.FindAsync(review.InquiryId);
        if (inquiry != null)
        {
            inquiry.Status = review.IsFeasible ? "FeasibilityApproved" : "Rejected";
            inquiry.UpdatedAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(int id, FeasibilityReview review)
    {
        if (id != review.Id) return BadRequest();

        _context.Entry(review).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.FeasibilityReviews.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var review = await _context.FeasibilityReviews.FindAsync(id);
        if (review == null) return NotFound();

        _context.FeasibilityReviews.Remove(review);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
