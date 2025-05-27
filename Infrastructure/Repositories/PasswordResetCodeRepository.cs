using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PasswordResetCodeRepository : IPasswordResetCodeRepository
    {
        private readonly ApplicationDbContext context;
        public PasswordResetCodeRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<PasswordResetCode?> GetByUserIdAsync(Guid userId)
        {
            return await context.PasswordResetCodes
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }
        public async Task<PasswordResetCode?> GetByCodeAsync(string code)
        {
            return await context.PasswordResetCodes
                .FirstOrDefaultAsync(x => x.Code == code);
        }
        public async Task<Result<Guid>> AddAsync(PasswordResetCode passwordResetCode)
        {
            try
            {
                await context.PasswordResetCodes.AddAsync(passwordResetCode);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(passwordResetCode.Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding password reset code: {ex.Message}");
            }
        }
        public async Task UpdateAsync(PasswordResetCode passwordResetCode)
        {
            context.PasswordResetCodes.Update(passwordResetCode);
            await context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var passwordResetCode =  await context.PasswordResetCodes.FindAsync(id);
            if (passwordResetCode != null)
            {
                context.PasswordResetCodes.Remove(passwordResetCode);
                await context.SaveChangesAsync();
            }
        }

        public async Task<PasswordResetCode?> GetByIdAsync(Guid id)
        {
            return await context.PasswordResetCodes
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<PasswordResetCode>> GetExpiredCodesAsync(DateTime expirationTime)
        {
            return await  context.Set<PasswordResetCode>()
            .Where(c => c.CreatedAt < expirationTime)
            .ToListAsync();
        }

        public async Task DeleteExpiredCodesAsync(List<PasswordResetCode> expiredCodes)
        {
            foreach (var code in expiredCodes)
            {
                context.PasswordResetCodes.Remove(code);
                await context.SaveChangesAsync();
            }
        }

        public Task<bool> ExistsAsync(string code)
        {
            return context.PasswordResetCodes
                .AnyAsync(x => x.Code == code);
        }
    }
}
