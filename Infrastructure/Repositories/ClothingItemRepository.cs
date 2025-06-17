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
                .Where(ci => ci.IsSold == false) // Exclude sold items
                .ToListAsync();
        }

        public async Task<IEnumerable<ClothingItem>> GetUnusedInLastSixMonthsAsync(Guid UserId)
        {
            var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);

            return await dbContext.ClothingItems
                .Include(ci => ci.Tags)
                .Where(ci => ci.LastWornDate == null || ci.LastWornDate < sixMonthsAgo && ci.UserId == UserId && ci.IsSold == false)
                .ToListAsync();
        }

        public async Task<ClothingItem?> GetByIdAsync(Guid id)
        {
            return await dbContext.ClothingItems
                .Include(ci => ci.Tags) // Eagerly load the Tags property
                .Where(ci => ci.IsSold == false)
                .FirstOrDefaultAsync(ci => ci.Id == id);
        }

        public async Task<Result<string>> UpdateAsync(ClothingItem clothingItem)
        {
            try
            {
                dbContext.ClothingItems.Update(clothingItem);
                await dbContext.SaveChangesAsync();
                return Result<string>.Success("Clothing item updated successfully.");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"{ex.Message}");
            }
        }

        public async Task<Dictionary<string, int>> GetCountByMaterialAsync(Guid UserId)
        {
            return await dbContext.ClothingItems
                .GroupBy(ci => ci.Material)
                .Where(g => g.Any(ci => ci.UserId == UserId) && g.All(ci => ci.IsSold == false)) // Filter by UserId
                .Select(g => new { Material = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Material ?? "Unknown", x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetMonthlyPurchaseCountAsync(Guid userId)
        {
            var currentYear = DateTime.UtcNow.Year;

            var monthlyData = await dbContext.ClothingItems
                .Where(ci => ci.UserId == userId &&
                             !ci.CreatedAt.Equals(null) &&
                             ci.CreatedAt.Year == currentYear &&
                             ci.IsSold == false)
                .GroupBy(ci => ci.CreatedAt.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var result = monthlyData.ToDictionary(
                x => new DateTime(currentYear, x.Month, 1).ToString("MMMM"),
                x => x.Count
            );

            return result;
        }



    }
}
