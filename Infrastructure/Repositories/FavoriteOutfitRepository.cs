using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class FavoriteOutfitRepository : IFavoriteOutfitRepository
    {
        private readonly ApplicationDbContext context;

        public FavoriteOutfitRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<Result<Guid>> AddAsync(FavoriteOutfit favoriteOutfit)
        {
            try
            {
                await context.FavoriteOutfits.AddAsync(favoriteOutfit);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(favoriteOutfit.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"Error adding favorite outfit: {ex.Message}");
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var favoriteOutfit = await context.FavoriteOutfits.FindAsync(id);
            if (favoriteOutfit != null)
            {
                context.FavoriteOutfits.Remove(favoriteOutfit);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<FavoriteOutfit>> GetAllByUserIdAsync(Guid userId)
        {
            return await context.FavoriteOutfits
                .Where(fo => fo.UserId == userId)
                .ToListAsync();
        }

        public async Task<FavoriteOutfit?> GetByIdAsync(Guid id)
        {
            return await context.FavoriteOutfits.FindAsync(id);
        }

        public async Task<FavoriteOutfit?> GetByUserAndOutfitAsync(Guid userId, Guid outfitId)
        {
            return await context.FavoriteOutfits
                .FirstOrDefaultAsync(fo => fo.UserId == userId && fo.OutfitId == outfitId);
        }
    }
}
