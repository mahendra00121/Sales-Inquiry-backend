using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyTrack.API.Data;
using PolyTrack.API.Models;

namespace PolyTrack.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public NotificationController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications()
    {
        return await _context.Notifications
            .Where(n => !n.IsRead) // Only show unread notifications
            .OrderByDescending(n => n.CreatedAt)
            .Take(20)
            .ToListAsync();
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        return await _context.Notifications.CountAsync(n => !n.IsRead);
    }

    [HttpPost("mark-as-read")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var unread = await _context.Notifications.Where(n => !n.IsRead).ToListAsync();
        foreach (var n in unread) n.IsRead = true;
        await _context.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPost("mark-as-read/{id}")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification == null) return NotFound();
        
        notification.IsRead = true;
        await _context.SaveChangesAsync();
        return Ok();
    }
}
