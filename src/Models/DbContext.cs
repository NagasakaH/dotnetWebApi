using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApi.Models;

public class AppDbContext : DbContext
{
  private readonly IConfiguration _configuration;

  public AppDbContext(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public required DbSet<User> Users { get; set; }
  public required DbSet<Role> Roles { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<User>().HasMany(e => e.Roles).WithMany(e => e.Users);
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    string appSqlServer = _configuration.GetValue<String>("SQLServer") ?? "sqlserver";
    string sqlServer = Environment.GetEnvironmentVariable("SQLServer") ?? appSqlServer;
    string appPort = _configuration.GetValue<String>("Port") ?? "1433";
    string port = Environment.GetEnvironmentVariable("Port") ?? appPort;
    string appDatabase = _configuration.GetValue<String>("Database") ?? "database";
    string database = Environment.GetEnvironmentVariable("Database") ?? appDatabase;
    string appId = _configuration.GetValue<String>("Id") ?? "sa";
    string id = Environment.GetEnvironmentVariable("Id") ?? appId;
    string appPassword = _configuration.GetValue<String>("Password") ?? "Password123";
    string password = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD") ?? appPassword;
    string connectionString =
      $"Server=tcp:{sqlServer},{port};Database={database};User Id={id};Password={password};TrustServerCertificate=True;";
    optionsBuilder.UseSqlServer(@connectionString);
  }
}
