namespace Application.DTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string Role { get; set; }
        public required string LoginProvider { get; set; }
        public string? GoogleId { get; set; } // sub from payload
        public string? ProfilePicture { get; set; } // from payload
    }
}
