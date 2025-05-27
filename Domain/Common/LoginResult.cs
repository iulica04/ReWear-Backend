namespace Domain.Common
{
    public class LoginResult
    {
        public required string Token { get; set; }
        public required string UserId { get; set; }
    }
}
