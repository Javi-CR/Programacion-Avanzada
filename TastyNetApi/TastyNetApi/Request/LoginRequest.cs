using System.ComponentModel.DataAnnotations;

namespace TastyNetApi.Request
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Password { get; set; }
    }

}
