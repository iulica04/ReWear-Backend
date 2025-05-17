namespace Application.Services
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool Verify(string hash, string password);
    }
}
