using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IClothingItemRepository
    {
        Task<ClothingItem?> GetByIdAsync(Guid id);
        Task<IEnumerable<ClothingItem>> GetAllAsync();
        Task<Result<Guid>> AddAsync(ClothingItem clothingItem);
        Task<Result<string>> UpdateAsync(ClothingItem clothingItem);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<ClothingItem>> GetUnusedInLastSixMonthsAsync(Guid UserId);
        Task<Dictionary<string, int>> GetCountByMaterialAsync(Guid UserId);
        Task<Dictionary<string, int>> GetMonthlyPurchaseCountAsync(Guid userId);
    }
}
