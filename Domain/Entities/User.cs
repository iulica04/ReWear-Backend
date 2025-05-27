namespace Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } 
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public  string? PasswordHash { get; set; }
        public string? Role { get; set; }
        public required string LoginProvider { get; set; }
        public string? GoogleId { get; set; } // sub from payload
        public string? ProfilePicture { get; set; } // from payload
    }
}
