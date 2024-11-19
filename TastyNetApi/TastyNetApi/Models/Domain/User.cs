namespace TastyNetApi.Models.Domain
{
    public class User
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? ProfileImage { get; set; }
        public long RoleId { get; set; }
        public DateTime CreatedUser { get; set; }
    }
}
