using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;


namespace TastyNetApi.Models
{
    [Index(nameof(Users.IdentificationNumber), IsUnique = true)]
    [Index(nameof(Users.Email), IsUnique = true)]
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string IdentificationNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [NotMapped]
        public string Token { get; set; } = string.Empty;

        public string? ProfilePicture { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "bit")]
        public bool Active { get; set; }

        [Required]
        [ForeignKey("RoleId")]
        [Column(TypeName = "smallint")]
        public short RoleId { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public bool UseTempPassword { get; set; }

        [Required]
        public DateTime Validity { get; set; }

        [Required]
        public int FailedAttempts { get; set; } = 0;

        public DateTime? LockedUntil { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public bool ValidatedEmail { get; set; }

        [Required]
        [MaxLength(100)]
        public string VerificationToken { get; set; } = string.Empty;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedUser { get; set; } = DateTime.Now;

        [JsonIgnore]
        public virtual Roles Role { get; set; }

    }
}
