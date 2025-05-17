using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ClothingItemRepository : IClothingItemRepository
    {
        private readonly ApplicationDbContext dbContext;
        public ClothingItemRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Result<Guid>> AddAsync(ClothingItem clothingItem)
        {
            try
            {
                await dbContext.ClothingItems.AddAsync(clothingItem);
                await dbContext.SaveChangesAsync();
                return Result<Guid>.Success(clothingItem.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"Error adding clothing item: {ex.Message}");
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var clothingItem = await dbContext.ClothingItems.FindAsync(id);
            if (clothingItem != null)
            {
                dbContext.ClothingItems.Remove(clothingItem);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ClothingItem>> GetAllAsync()
        {
            return await dbContext.ClothingItems
                .Include(ci => ci.Tags) // Eagerly load the Tags property
                .ToListAsync();
        }

        public async Task<ClothingItem?> GetByIdAsync(Guid id)
        {
            return await dbContext.ClothingItems
                .Include(ci => ci.Tags) // Eagerly load the Tags property
                .FirstOrDefaultAsync(ci => ci.Id == id);
        }

        public Task UpdateAsync(ClothingItem clothingItem)
        {
            dbContext.ClothingItems.Update(clothingItem);
            return dbContext.SaveChangesAsync();
        }
    }
}
