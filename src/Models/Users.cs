using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Models
{
  [Index(nameof(Username), IsUnique = true)]
  public class User
  {
    [Key]
    public int UserId { get; set; }

    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public virtual List<Role> Roles { get; set; } = new();
  }
}
