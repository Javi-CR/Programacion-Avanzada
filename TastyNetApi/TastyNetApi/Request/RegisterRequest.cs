using System.ComponentModel.DataAnnotations;

namespace TastyNetApi.Request
{
    public class RegisterRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "IdentificationNumber must be a positive integer.")]
        public int IdentificationNumber { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Name can't be longer than 255 characters.")]
        public string Name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Password { get; set; }

    }


}
