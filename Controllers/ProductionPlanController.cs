using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyTrack.API.Data;
using PolyTrack.API.Models;

namespace PolyTrack.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductionPlanController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductionPlanController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductionPlan>>> GetPlans()
    {
        return await _context.ProductionPlans.Include(p => p.Order).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductionPlan>> GetPlan(int id)
    {
        var plan = await _context.ProductionPlans.Include(p => p.Order).FirstOrDefaultAsync(p => p.Id == id);
        if (plan == null) return NotFound();
        return plan;
    }

    [HttpPost]
    public async Task<ActionResult<ProductionPlan>> CreatePlan(ProductionPlan plan)
    {
        _context.ProductionPlans.Add(plan);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPlan), new { id = plan.Id }, plan);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlan(int id, ProductionPlan plan)
    {
        if (id != plan.Id) return BadRequest();

        _context.Entry(plan).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.ProductionPlans.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlan(int id)
    {
        var plan = await _context.ProductionPlans.FindAsync(id);
        if (plan == null) return NotFound();

        _context.ProductionPlans.Remove(plan);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
