using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
  private readonly ILogger<UserController> _logger;
  private readonly AppDbContext _context;
  private readonly ITokenService _tokenService;

  public UserController(
    ILogger<UserController> logger,
    AppDbContext context,
    ITokenService tokenService
  )
  {
    _logger = logger;
    _context = context;
    _tokenService = tokenService;
  }

  [HttpPost]
  public async Task<ActionResult<User>> PostUser(User user)
  {
    var hasher = new PasswordHasher<User>();
    user.Password = hasher.HashPassword(user, user.Password);
    _context.Users.Add(user);
    await _context.SaveChangesAsync();
    return CreatedAtAction("GetUser", new { id = user.UserId }, user);
  }

  public class LoginRequest
  {
    public required string UserName { get; set; }
    public required string Password { get; set; }
  }


  [HttpPost("login")]
  public async Task<ActionResult<string>> Login(LoginRequest request)
  {
    var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);

    if (user == null)
    {
      return NotFound();
    }

    var hasher = new PasswordHasher<User>();
    var result = hasher.VerifyHashedPassword(user, user.Password, request.Password);

    if (result == PasswordVerificationResult.Failed)
    {
      return Unauthorized();
    }

    return _tokenService.GenerateToken(user);
  }
}
