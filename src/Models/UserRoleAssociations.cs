using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Models
{
  public class UserRoleAssociation
  {
    [Key]
    public int UserRoleAssociationId { get; set; }

    [ForeignKey(nameof(Role))]
    public required int RoleId { get; set; }
    [ForeignKey(nameof(User))]
    public required int UserId { get; set; }
  }
}
