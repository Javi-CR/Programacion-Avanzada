using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TastyNetApi.Request
{
    public class VerifyTokenRequest
    {
        [Required]
        [MaxLength(100)]
        public string VerificationToken { get; set; }
    }
}
