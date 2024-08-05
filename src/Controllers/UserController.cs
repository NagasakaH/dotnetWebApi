using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Controllers;
using WebApi.Helpers;
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
    public required string Username { get; set; }
    public required string Password { get; set; }
  }

  public class UserResponse
  {
    public int UserId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }

    public List<RoleController.RoleResponse> Roles { get; set; } = new();

    public static implicit operator UserResponse(User user)
    {
      return GenericConverter.ConvertObject<UserResponse>(user);
    }
  }

  private async Task<User?> getAuthorizedUser()
  {
    User? currentUser = null;
    var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c =>
      c.Type == ClaimTypes.NameIdentifier || c.Type == JwtRegisteredClaimNames.NameId
    );
    if (userIdClaim != null)
    {
      var userId = int.Parse(userIdClaim.Value);
      currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }
    return currentUser;
  }

  [AllowAnonymous]
  [HttpPost("login")]
  public async Task<ActionResult<string>> Login(LoginRequest request)
  {
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

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

    // クッキー発行
    var claims = new List<Claim>
    {
      new Claim(ClaimTypes.Name, user.Username),
      new Claim(ClaimTypes.Email, user.Email),
      new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
    };
    var claimsIdentity = new ClaimsIdentity(
      claims,
      CookieAuthenticationDefaults.AuthenticationScheme
    );
    await HttpContext.SignInAsync(
      CookieAuthenticationDefaults.AuthenticationScheme,
      new ClaimsPrincipal(claimsIdentity)
    );

    return _tokenService.GenerateToken(user);
  }

  [HttpPost("logout")]
  public async Task<IActionResult> Logout()
  {
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Ok();
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
  [HttpGet("me")]
  public async Task<ActionResult<UserResponse>> GetMe()
  {
    User? currentUser = await getAuthorizedUser();
    if(currentUser == null){
      return Unauthorized();
    }
    return (UserResponse)currentUser;

  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
  [HttpGet("{id}")]
  public async Task<ActionResult<UserResponse>> GetUser(long id)
  {
    // 現在のユーザーを取得して同一かどうか確認
    User? currentUser = await getAuthorizedUser();
    if(currentUser == null || currentUser.UserId != id){
      return Unauthorized();
    }

    var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
    if (user == null)
    {
      return NotFound();
    }
    UserResponse response = user;
    var userRoleAssociations = await _context
      .UserRoleAssociations.Where(ur => ur.UserId == user.UserId)
      .ToListAsync();
    foreach (var userRoleAssociation in userRoleAssociations)
    {
      var role = await _context.Roles.FirstOrDefaultAsync(r =>
        r.RoleId == userRoleAssociation.RoleId
      );
      if (role == null)
      {
        return NotFound();
      }
      RoleController.RoleResponse roleResponse = role;
      response.Roles.Add(role);
    }
    return response;
  }

  [Authorize]
  [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
  [HttpPut("{id}")]
  public async Task<ActionResult<UserResponse>> UpdateUser(long id)
  {
    var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
    if (user == null)
    {
      return NotFound();
    }
    UserResponse response = user;
    return response;
  }
}
