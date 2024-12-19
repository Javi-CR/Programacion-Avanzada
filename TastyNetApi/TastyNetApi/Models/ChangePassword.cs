using System.ComponentModel.DataAnnotations.Schema;

namespace TastyNetApi.Models
{

    [NotMapped]
    public class ChangePassword
    {
        public long Id { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}
