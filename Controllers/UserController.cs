using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyTrack.API.Data;
using PolyTrack.API.Models;

namespace PolyTrack.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.Users
            .Select(u => new User 
            { 
                Id = u.Id, 
                Username = u.Username, 
                FullName = u.FullName, 
                Role = u.Role, 
                CreatedAt = u.CreatedAt 
            })
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        if (await _context.Users.AnyAsync(u => u.Username == user.Username))
        {
            return BadRequest("Username already exists");
        }

        if (string.IsNullOrEmpty(user.Password))
        {
            return BadRequest("Password is required for new users");
        }

        user.CreatedAt = DateTime.UtcNow;
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, User user)
    {
        if (id != user.Id) return BadRequest();

        var existingUser = await _context.Users.FindAsync(id);
        if (existingUser == null) return NotFound();

        existingUser.FullName = user.FullName;
        existingUser.Role = user.Role;
        if (!string.IsNullOrEmpty(user.Password))
        {
            existingUser.Password = user.Password;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();
        if (user.Username == "admin") return BadRequest("Cannot delete the main admin");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
