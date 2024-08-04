using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("roles")]
public class RoleController : ControllerBase
{
  private readonly ILogger<RoleController> _logger;
  private readonly AppDbContext _context;
  private readonly ITokenService _tokenService;

  public RoleController(
    ILogger<RoleController> logger,
    AppDbContext context,
    ITokenService tokenService
  )
  {
    _logger = logger;
    _context = context;
    _tokenService = tokenService;
  }

  public class RoleResponse
  {
    public int RoleId { get; set; }
    public required string RoleName { get; set; }

    public static implicit operator RoleResponse(Role role)
    {
      return GenericConverter.ConvertObject<RoleResponse>(role);
    }
  }

  [Authorize]
  [HttpGet]
  public async Task<ActionResult<List<RoleResponse>>> GetRoles(int page, int limit)
  {
    // TODO ヘッダにページネーションの設定を渡して返却する
    if (page ==0){
      page = 1;
    }
    if (limit ==0) {
      limit = 100;
    }

    var roles = await _context.Roles.OrderBy(b => b.RoleId).Take(limit).ToListAsync();
    var response = GenericConverter.ConvertList<RoleResponse>(roles);
    return response;
  }

  [Authorize]
  [HttpGet("{id}")]
  public async Task<ActionResult<RoleResponse>> GetRole(long id)
  {
    // 必要に応じて制限をかける

    var role = await _context.Roles.FirstOrDefaultAsync(u => u.RoleId == id);
    if (role == null)
    {
      return NotFound();
    }
    RoleResponse response = role;
    return response;
  }
}
