using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IUserRepository
    {
        Task<Result<Guid>> AddAsync(User user);
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUserNameAsync(string userName);
        Task<IEnumerable<User>> GetAllAsync();
        Task UpdateAsync(User user);
        Task  DeleteAsync(Guid id);
        Task<bool> UserNameExistsAsync(string userName);
        Task<bool> EmailExistsAsync(string email);
        Task<LoginResult?> Login(string email, string password);
        Task<LoginResult?> LoginWithGoogle(string email, string googleId);
    }
}
