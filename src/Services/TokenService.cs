using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WebApi.Models;

namespace WebApi.Services;

public interface ITokenService
{
  string GenerateToken(User user);
}

public class TokenService : ITokenService
{
  private readonly IConfiguration _configuration;
  private readonly ILogger<TokenService> _logger;

  public TokenService(
    IConfiguration configuration,
    ILogger<TokenService> logger
  )
  {
    _configuration = configuration;
    _logger = logger;
  }

  public class JwtOptions
  {
    public required string[] Issuer { get; set; }
    public required string[] Audience { get; set; }
    public required string Key { get; set; }
    public int ExpirationMinutes { get; set; }
  }

  public static JwtOptions GetJwtOptions(IConfiguration configuration)
  {
    JwtOptions jwtOptions =
      configuration.GetValue<JwtOptions>("JwtOptions")
      ?? new JwtOptions()
      {
        Issuer = new string[] { },
        Audience = new string[] { },
        Key = "TX7TWzhNSBS6Hjf1BAHArx8V0NkH96G4eCyhHMXPrYjJy+kl9Z3xmVpzKrbt6cSR",
        ExpirationMinutes = 60
      };
    return jwtOptions;
  }

  public static void AddAuthService(IServiceCollection services, IConfiguration configuration)
  {
    JwtOptions jwtOptions = GetJwtOptions(configuration);

    services
      .AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(o =>
      {
        o.TokenValidationParameters = new TokenValidationParameters
        {
          ValidIssuer = "http://localhost:5000",
          ValidAudience = "http://localhost:5000",
          ValidateIssuer = false,
          ValidateAudience = false,
          ClockSkew = TimeSpan.Zero,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
        };
      });
    services.AddAuthorization();
  }

  public string GenerateToken(User user)
  {
    JwtOptions jwtOptions = GetJwtOptions(_configuration);
    JwtSecurityTokenHandler tokenHandler = new();
    JwtSecurityToken jwtSecurityToken = tokenHandler.CreateJwtSecurityToken();
    return tokenHandler.WriteToken(jwtSecurityToken);
  }
}
