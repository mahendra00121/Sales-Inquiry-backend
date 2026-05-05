using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyTrack.API.Data;
using PolyTrack.API.Models;
using Microsoft.AspNetCore.Authorization;

namespace PolyTrack.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MasterDataController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MasterDataController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/MasterData
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MasterData>>> GetMasterData([FromQuery] string? category = null)
    {
        var query = _context.MasterData.AsQueryable();

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(m => m.Category == category);
        }

        return await query.ToListAsync();
    }

    // GET: api/MasterData/5
    [HttpGet("{id}")]
    public async Task<ActionResult<MasterData>> GetMasterDataById(int id)
    {
        var masterData = await _context.MasterData.FindAsync(id);

        if (masterData == null)
        {
            return NotFound();
        }

        return masterData;
    }

    // POST: api/MasterData
    [HttpPost]
    public async Task<ActionResult<MasterData>> PostMasterData(MasterData masterData)
    {
        masterData.CreatedAt = DateTime.Now;
        _context.MasterData.Add(masterData);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMasterDataById), new { id = masterData.Id }, masterData);
    }

    // PUT: api/MasterData/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutMasterData(int id, MasterData masterData)
    {
        if (id != masterData.Id)
        {
            return BadRequest();
        }

        masterData.UpdatedAt = DateTime.Now;
        _context.Entry(masterData).State = EntityState.Modified;
        // Don't modify CreatedAt
        _context.Entry(masterData).Property(x => x.CreatedAt).IsModified = false;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MasterDataExists(id))
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

    // DELETE: api/MasterData/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMasterData(int id)
    {
        var masterData = await _context.MasterData.FindAsync(id);
        if (masterData == null)
        {
            return NotFound();
        }

        _context.MasterData.Remove(masterData);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool MasterDataExists(int id)
    {
        return _context.MasterData.Any(e => e.Id == id);
    }
}
