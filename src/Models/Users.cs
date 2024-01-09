using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Models
{
  [Index(nameof(UserName), IsUnique = true)]
  public class User
  {
    [Key]
    public int UserId { get; set; }

    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public List<Role> Roles { get; set; } = new();
  }
}
