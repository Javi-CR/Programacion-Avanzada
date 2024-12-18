namespace TastyNet.Models
{
    public class ChangePassword
    {
        public long Id { get; set; }
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
