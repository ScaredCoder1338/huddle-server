using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HuddleServer.Data;
using HuddleServer.Models;
using System.Security.Cryptography;
using System.Text;

namespace HuddleServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            // Проверяем существует ли пользователь
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Email);

            if (existingUser != null)
            {
                return BadRequest(new { message = "Пользователь уже существует" });
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = request.PasswordHash,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { id = user.Id, username = user.Username, email = user.Email });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Ошибка регистрации: {ex.Message}" });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.PasswordHash == request.PasswordHash);

            if (user == null)
            {
                return Unauthorized(new { message = "Неверный email или пароль" });
            }

            return Ok(new { id = user.Id, username = user.Username, email = user.Email });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Ошибка входа: {ex.Message}" });
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchUsers([FromQuery] string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Ok(new List<string>());
            }

            var users = await _context.Users
                .Where(u => u.Username.Contains(query))
                .Select(u => u.Username)
                .Take(10)
                .ToListAsync();

            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Ошибка поиска: {ex.Message}" });
        }
    }

    [HttpGet("user/{username}")]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound(new { message = "Пользователь не найден" });
            }

            return Ok(new { id = user.Id, username = user.Username });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Ошибка: {ex.Message}" });
        }
    }
}

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}
