using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IClothingItemRepository
    {
        Task<ClothingItem?> GetByIdAsync(Guid id);
        Task<IEnumerable<ClothingItem>> GetAllAsync();
        Task<Result<Guid>> AddAsync(ClothingItem clothingItem);
        Task UpdateAsync(ClothingItem clothingItem);
        Task DeleteAsync(Guid id);
    }
}
