using Application.DTOs;
using Application.Use_Cases.Queries.FavoriteOutfitQueries;
using Application.Utils;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Gridify;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.FavoriteOutfitQueryHandlers
{
    public class GetFavoriteOutfitsByUserIdQueryHandler : IRequestHandler<GetFavoriteOutfitsByUserIdQuery, Result<PagedResult<OutfitDTO>>>
    {
        private readonly IFavoriteOutfitRepository repository;
        private readonly IOutfitRepository outfitRepository;

        public GetFavoriteOutfitsByUserIdQueryHandler(IFavoriteOutfitRepository repository, IOutfitRepository outfitRepository)
        {
            this.repository = repository;
            this.outfitRepository = outfitRepository;
        }
        public async Task<Result<PagedResult<OutfitDTO>>> Handle(GetFavoriteOutfitsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var favoriteOutfits = await repository.GetAllByUserIdAsync(request.UserId);

            if (favoriteOutfits == null || !favoriteOutfits.Any())
            {
                return Result<PagedResult<OutfitDTO>>.Success(new PagedResult<OutfitDTO>(new List<OutfitDTO>(), 0));
            }

            var outfits = new List<OutfitDTO>();
            foreach (var favoriteOutfit in favoriteOutfits)
            {
                var outfit = await outfitRepository.GetByIdAsync(favoriteOutfit.OutfitId);
                if (outfit != null)
                {
                    outfits.Add(new OutfitDTO
                    {
                        Id = outfit.Id,
                        UserId = outfit.UserId,
                        Name = outfit.Name,
                        CreatedAt = outfit.CreatedAt,
                        Season = outfit.Season,
                        Description = outfit.Description,
                        ImageUrl = outfit.ImageUrl,
                        ClothingItemDTOs = outfit.OutfitClothingItems
                        .Select(oci => oci.ClothingItem)
                        .Where(ci => ci != null)
                        .Select(ci => new ClothingItemDTO
                        {
                            Id = ci.Id,
                            UserId = ci.UserId,
                            Name = ci.Name,
                            Category = ci.Category,
                            Color = ci.Color,
                            Brand = ci.Brand,
                            Material = ci.Material,
                            PrintType = ci.PrintType,
                            PrintDescription = ci.PrintDescription,
                            Description = ci.Description,
                            FrontImageUrl = ci.FrontImageUrl,
                            BackImageUrl = ci.BackImageUrl,
                            NumberOfWears = ci.NumberOfWears
                        }).ToList()
                    });
                }
            }

            var totalCount = outfits.Count;
            var pagedItems = outfits.AsQueryable().ApplyPaging(request.Page, request.PageSize);
            var pagedResult = new PagedResult<OutfitDTO>(pagedItems.ToList(), totalCount);
            return Result<PagedResult<OutfitDTO>>.Success(pagedResult);

        }
    }
}
