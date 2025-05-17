namespace Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } 
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public string? Role { get; set; }
    }
}
