using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers;


[ApiController]
[Route("users")]
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

  [Authorize]
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


  [AllowAnonymous]
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

  [Authorize]
  [HttpGet("{id}")]
  public async Task<ActionResult<User>> GetUser(long id)
  {
    // 必要に応じて制限をかける

    var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
    if (user == null)
    {
      return NotFound();
    }

    return user;
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<User>> UpdateUser(long id)
  {
    var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
    if (user == null)
    {
      return NotFound();
    }
    return user;
  }
}
