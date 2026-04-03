using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyTrack.API.Data;
using PolyTrack.API.Models;

namespace PolyTrack.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SalesOrderController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SalesOrderController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SalesOrder>>> GetOrders()
    {
        return await _context.SalesOrders.Include(o => o.Inquiry).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SalesOrder>> GetOrder(int id)
    {
        var order = await _context.SalesOrders.Include(o => o.Inquiry).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();
        return order;
    }

    [HttpPost]
    public async Task<ActionResult<SalesOrder>> CreateOrder(SalesOrder order)
    {
        order.OrderNumber = "SO-" + DateTime.Now.Year + "-" + (await _context.SalesOrders.CountAsync() + 1).ToString("D5");
        
        _context.SalesOrders.Add(order);

        var inquiry = await _context.SalesInquiries.FindAsync(order.InquiryId);
        if (inquiry != null)
        {
            inquiry.Status = "Ordered";
            inquiry.UpdatedAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, SalesOrder order)
    {
        if (id != order.Id) return BadRequest();

        _context.Entry(order).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.SalesOrders.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.SalesOrders.FindAsync(id);
        if (order == null) return NotFound();

        _context.SalesOrders.Remove(order);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
