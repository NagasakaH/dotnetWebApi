using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
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
    // Set Relations
    modelBuilder.Entity<User>().HasMany(e => e.Roles).WithMany(e => e.Users);
    // Insert Seed Data
    modelBuilder.Entity<Role>().HasData(new Role { RoleId = 1, RoleName = "Admin" });
    User admin = new User
    {
      UserId = 1,
      UserName = "admin",
      Password = "",
      Email = "example@example.com",
    };
    admin.Password = new PasswordHasher<User>().HashPassword(admin, "admin");
    modelBuilder.Entity<User>().HasData(admin);
    modelBuilder.Entity("RoleUser").HasData(new { UsersUserId = 1, RolesRoleId = 1 });
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    string host =
      Environment.GetEnvironmentVariable("POSTGRES_HOST")
      ?? _configuration.GetValue<String>("DatabaseHost")
      ?? "postgres";
    string port =
      Environment.GetEnvironmentVariable("POSTGRES_PORT")
      ?? _configuration.GetValue<String>("DatabasePort")
      ?? "5432";
    string database =
      Environment.GetEnvironmentVariable("POSTGRES_DB")
      ?? _configuration.GetValue<String>("Database")
      ?? "test01";
    string username =
      Environment.GetEnvironmentVariable("POSTGRES_USER")
      ?? _configuration.GetValue<String>("DatabaseUser")
      ?? "test01";
    string password =
      Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")
      ?? _configuration.GetValue<String>("DatabasePassword")
      ?? "test01";
    string connectionString =
      $"Host={host};Database={database};Username={username};Password={password}";
    optionsBuilder.UseNpgsql(@connectionString);
  }
}
