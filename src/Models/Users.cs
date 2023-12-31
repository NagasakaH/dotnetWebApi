using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
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
