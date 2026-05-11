using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyTrack.API.Data;
using PolyTrack.API.Models;
using PolyTrack.API.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PolyTrack.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthController(ApplicationDbContext context, IConfiguration configuration, IEmailService emailService)
    {
        _context = context;
        _configuration = configuration;
        _emailService = emailService;
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

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "PolyTrackSuperSecretKey@1234567890_ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role ?? "User")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new { 
            message = "Login successful", 
            token = tokenString,
            user = new {
                userId = user.Id, 
                username = user.Username,
                role = user.Role 
            }
        });
    }

    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
        {
            return Unauthorized();
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            id = user.Id,
            username = user.Username,
            fullName = user.FullName,
            role = user.Role,
            createdAt = user.CreatedAt
        });
    }

    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
        {
            return Unauthorized();
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(request.FullName))
        {
            user.FullName = request.FullName;
        }

        if (!string.IsNullOrEmpty(request.Password))
        {
            user.Password = request.Password; // In a real app, hash this!
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Profile updated successfully" });
    }

    // --- Forgot Password Logic ---

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
        {
            // For security, don't reveal if user exists. But in internal ERP, it's fine.
            return BadRequest(new { message = "Email not found" });
        }

        // Generate 6-digit OTP
        string otp = new Random().Next(100000, 999999).ToString();
        user.ResetOtp = otp;
        user.ResetOtpExpiry = DateTime.UtcNow.AddMinutes(15);

        await _context.SaveChangesAsync();

        // Send Email
        var subject = "Security Verification Code: " + otp + " - PolyTrack ERP";
        var body = $@"
            <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e2e8f0; border-radius: 16px; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);'>
                <div style='background-color: #269896; padding: 30px; text-align: center;'>
                    <h1 style='color: white; margin: 0; font-size: 24px; letter-spacing: 2px; font-weight: 900;'>INDAS ANALYTICS</h1>
                    <p style='color: #ccfbf1; margin: 5px 0 0; font-size: 12px; font-weight: 600; text-transform: uppercase; letter-spacing: 1px;'>PolyTrack ERP Security</p>
                </div>
                <div style='padding: 40px; background-color: #ffffff;'>
                    <h2 style='color: #0f172a; margin: 0 0 20px; font-size: 20px; font-weight: 700;'>Account Verification</h2>
                    <p style='color: #475569; line-height: 1.6; margin-bottom: 30px;'>Hello <b>{user.FullName}</b>,<br><br>We received a request to access your PolyTrack account (<b>@{user.Username}</b>). Please use the following security code to verify your identity. This code is valid for 15 minutes.</p>
                    
                    <div style='background-color: #f8fafc; border: 2px dashed #cbd5e1; border-radius: 12px; padding: 25px; text-align: center; margin-bottom: 30px;'>
                        <span style='display: block; color: #64748b; font-size: 11px; font-weight: 800; text-transform: uppercase; margin-bottom: 10px; letter-spacing: 1px;'>Your Verification Code</span>
                        <span style='font-family: monospace; font-size: 42px; font-weight: 900; color: #269896; letter-spacing: 8px; line-height: 1;'>{otp}</span>
                    </div>

                    <p style='color: #64748b; font-size: 13px; line-height: 1.6;'>If you did not make this request, you can safely ignore this email. Your account security has not been compromised.</p>
                </div>
                <div style='background-color: #f1f5f9; padding: 20px; text-align: center;'>
                    <p style='color: #94a3b8; font-size: 11px; margin: 0;'>&copy; {DateTime.Now.Year} Indas Analytics. All rights reserved.<br>This is an automated security notification.</p>
                </div>
            </div>";

        await _emailService.SendEmailAsync(user.Email!, subject, body);

        return Ok(new { message = "Verification code sent to your email" });
    }

    [HttpPost("verify-reset-otp")]
    public async Task<IActionResult> VerifyResetOtp([FromBody] VerifyOtpRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.ResetOtp == request.Otp);
        
        if (user == null || user.ResetOtpExpiry < DateTime.UtcNow)
        {
            return BadRequest(new { message = "Invalid or expired verification code" });
        }

        return Ok(new { message = "OTP verified. You can now reset your password." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.ResetOtp == request.Otp);

        if (user == null || user.ResetOtpExpiry < DateTime.UtcNow)
        {
            return BadRequest(new { message = "Verification failed. Please try again." });
        }

        user.Password = request.NewPassword; // In real apps, hash this
        user.ResetOtp = null; // Clear OTP
        user.ResetOtpExpiry = null;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Password reset successfully. Please login with your new password." });
    }

    // Temporary endpoint to create a default admin user
    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        if (await _context.Users.AnyAsync()) return BadRequest("Users already exist");

        var admin = new User { 
            Username = "admin", 
            Password = "adminPassword", 
            FullName = "Admin User", 
            Role = "Admin",
            Email = "admin@polytrack.com" 
        };
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

public class UpdateProfileRequest
{
    public string? FullName { get; set; }
    public string? Password { get; set; }
}

public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}

public class VerifyOtpRequest
{
    public string Email { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
}

public class ResetPasswordRequest
{
    public string Email { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
