using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyTrack.API.Data;
using PolyTrack.API.Models;

namespace PolyTrack.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.Password == request.Password);

        if (user == null)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        return Ok(new { 
            message = "Login successful", 
            userId = user.Id, 
            username = user.Username,
            role = user.Role 
        });
    }

    // Temporary endpoint to create a default admin user
    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        if (await _context.Users.AnyAsync()) return BadRequest("Users already exist");

        var admin = new User { Username = "admin", Password = "adminPassword", FullName = "Admin User", Role = "Admin" };
        _context.Users.Add(admin);
        await _context.SaveChangesAsync();

        return Ok("Admin user created successfully");
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
