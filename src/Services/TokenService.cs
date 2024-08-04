using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
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

  public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
  {
    _configuration = configuration;
    _logger = logger;
  }

  public class JwtOptions
  {
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required SymmetricSecurityKey Key { get; set; }
    public int ExpirationMinutes { get; set; }
  }

  public static JwtOptions GetJwtOptions(IConfiguration configuration)
  {
    var key =
      Environment.GetEnvironmentVariable("JWT_SECRET")
      ?? configuration.GetValue<string>("JWTSecretKey")
      ?? "TX7TWzhNSBS6Hjf1BAHArx8V0NkH96G4eCyhHMXPrYjJy+kl9Z3xmVpzKrbt6cSR";
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    JwtOptions jwtOptions =
      configuration.GetValue<JwtOptions>("JwtOptions")
      ?? new JwtOptions()
      {
        Issuer =
          Environment.GetEnvironmentVariable("JWT_ISSUER")
          ?? configuration.GetValue<string>("JWTIssuer")
          ?? "issuer",
        Audience =
          Environment.GetEnvironmentVariable("JWT_AUDIENCE")
          ?? configuration.GetValue<string>("JWTAudience")
          ?? "audience",
        ExpirationMinutes = 60,
        Key = securityKey
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
          ValidIssuer = jwtOptions.Issuer,
          ValidAudience = jwtOptions.Audience,
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateIssuerSigningKey = true,
          ClockSkew = TimeSpan.Zero,
          IssuerSigningKey = jwtOptions.Key
        };
        o.SaveToken = true;
      });
    services.AddAuthorization();
  }

  public string GenerateToken(User user)
  {
    JwtOptions jwtOptions = GetJwtOptions(_configuration);
    var credentials = new SigningCredentials(jwtOptions.Key, "HS256");
    var claims = new[]
    {
      new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
      new Claim(JwtRegisteredClaimNames.Name, user.Username),
      new Claim(JwtRegisteredClaimNames.Email, user.Email)
    };
    JwtSecurityTokenHandler tokenHandler = new();
    JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
      issuer: jwtOptions.Issuer,
      audience: jwtOptions.Audience,
      claims: claims,
      expires: DateTime.Now.AddMinutes(jwtOptions.ExpirationMinutes),
      signingCredentials: credentials
    );
    return tokenHandler.WriteToken(jwtSecurityToken);
  }
}
