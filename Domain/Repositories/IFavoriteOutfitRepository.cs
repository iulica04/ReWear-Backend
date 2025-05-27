using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IFavoriteOutfitRepository 
    {
        Task<Result<Guid>> AddAsync(FavoriteOutfit favoriteOutfit);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<FavoriteOutfit>> GetAllByUserIdAsync(Guid userId);
        Task<FavoriteOutfit?> GetByIdAsync(Guid id);
        Task<FavoriteOutfit?> GetByUserAndOutfitAsync(Guid userId, Guid outfitId);
    }
}
