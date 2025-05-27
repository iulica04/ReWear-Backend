using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IPasswordResetCodeRepository
    {
        Task<PasswordResetCode?> GetByUserIdAsync(Guid userId);
        Task<PasswordResetCode?> GetByCodeAsync(string code);
        Task<PasswordResetCode?> GetByIdAsync(Guid id);
        Task<Result<Guid>> AddAsync(PasswordResetCode passwordResetCode);
        Task UpdateAsync(PasswordResetCode passwordResetCode);
        Task DeleteAsync(Guid id);
        Task<List<PasswordResetCode>> GetExpiredCodesAsync(DateTime expirationTime);
        Task DeleteExpiredCodesAsync(List<PasswordResetCode> expiredCodes);
        Task<bool> ExistsAsync(string code);
    }

}
