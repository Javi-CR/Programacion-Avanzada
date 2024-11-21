namespace TastyNet.Models
{
    public class Users
    {
        public long Id { get; set; }
        public string IdentificationNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        //public string ConfirmarPassword { get; set; } = string.Empty;
        public short RoleId { get; set; }
        public string RolName { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;

    }

}