namespace MiniBBS.Models
{
    public class AdminUserViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Roles { get; set; }
    }
}
