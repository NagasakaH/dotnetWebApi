using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext to the container.
builder.Services.AddDbContext<AppDbContext>();

// Add Authentication and Authorization to the container.
builder.Services.AddSingleton<ITokenService, TokenService>();
TokenService.AddAuthService(builder.Services, builder.Configuration);
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        options.Events.OnRedirectToLogin = cxt =>
        {
            // ログイン画面に遷移するのではなくエラーを返す
            cxt.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = cxt =>
        {
            // エラー画面に遷移するのではなくエラーを返す
            cxt.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToLogout = cxt => Task.CompletedTask;
    });

// Add Controllers to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
if (true)
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
