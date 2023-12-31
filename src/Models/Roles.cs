using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
  public class Role
  {
    [Key]
    public int RoleId { get; set; }
    public required string RoleName { get; set; }
    public List<User> Users { get; set; } = new();
  }
}
