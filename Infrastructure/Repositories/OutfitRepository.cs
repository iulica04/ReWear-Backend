using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class OutfitRepository : IOutfitRepository
    {
        private readonly ApplicationDbContext context;
        public OutfitRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Result<Guid>> AddAsync(Outfit outfit)
        {
            try
            {
                await context.Outfits.AddAsync(outfit);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(outfit.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"Error adding outfit: {ex.Message}");
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var outfit = await context.Outfits.FindAsync(id);
            if (outfit != null)
            {
                context.Outfits.Remove(outfit);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Outfit>> GetAllAsync()
        {
            return await context.Outfits
                .Include(o => o.OutfitClothingItems)
                    .ThenInclude(oci => oci.ClothingItem)
                .ToListAsync();
        }

        public async Task<Outfit?> GetByIdAsync(Guid id)
        {
            return await context.Outfits
                .Include(o => o.OutfitClothingItems)
                    .ThenInclude(oci => oci.ClothingItem)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public Task UpdateAsync(Outfit outfit)
        {
            context.Outfits.Update(outfit);
            return context.SaveChangesAsync();
        }
    }
}
