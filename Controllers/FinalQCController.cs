using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyTrack.API.Data;
using PolyTrack.API.Models;

namespace PolyTrack.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FinalQCController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FinalQCController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FinalQC>>> GetQCReports()
    {
        return await _context.FinalQCReports.Include(q => q.ShopFloorRecord).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FinalQC>> GetQCReport(int id)
    {
        var report = await _context.FinalQCReports.Include(q => q.ShopFloorRecord).FirstOrDefaultAsync(q => q.Id == id);
        if (report == null) return NotFound();
        return report;
    }

    [HttpPost]
    public async Task<ActionResult<FinalQC>> SubmitQCReport(FinalQC report)
    {
        _context.FinalQCReports.Add(report);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetQCReport), new { id = report.Id }, report);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQCReport(int id, FinalQC report)
    {
        if (id != report.Id) return BadRequest();

        _context.Entry(report).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.FinalQCReports.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQCReport(int id)
    {
        var report = await _context.FinalQCReports.FindAsync(id);
        if (report == null) return NotFound();

        _context.FinalQCReports.Remove(report);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
