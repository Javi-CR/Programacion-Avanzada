using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TastyNetApi.Request
{
    public class ProfileRequest
    {
        [Required(ErrorMessage = "Identificator is needed")]
        public long Id { get; set; }

        [StringLength(255, ErrorMessage = "Name can't be longer than 255 characters.")]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        public string ProfilePicture { get; set; } = string.Empty;
    }

    public class AccessRecoveryRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [MaxLength(100)]
        public string Email { get; set; }
    }

}
