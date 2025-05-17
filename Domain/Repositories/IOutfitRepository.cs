using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IOutfitRepository
    {
        Task<Result<Guid>> AddAsync(Outfit outfit);
        Task UpdateAsync(Outfit outfit);
        Task DeleteAsync(Guid id);
        Task<Outfit?> GetByIdAsync(Guid id);
        Task<IEnumerable<Outfit>> GetAllAsync();
    }
}
