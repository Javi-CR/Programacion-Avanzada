using System.ComponentModel.DataAnnotations;

namespace TastyNetApi.Request
{
    public class LoginRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

}
