using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TastyNetApi.Models;

[Index(nameof(Roles.RolName), IsUnique = true)]
public class Roles
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column(TypeName = "smallint")]
    public short Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string RolName { get; set; } = string.Empty;  
}