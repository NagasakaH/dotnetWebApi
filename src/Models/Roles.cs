using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Models
{
  [Index(nameof(RoleName), IsUnique = true)]
  public class Role
  {
    [Key]
    public int RoleId { get; set; }
    public required string RoleName { get; set; }
    public virtual List<User> Users { get; set; } = new();
  }
}
