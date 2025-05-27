namespace Domain.Entities
{
    public class PasswordResetCode
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string Code { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int Attempts { get; set; }
        public bool IsUsed { get; set; }

    }
}
