using Application.DTOs;
using Application.Use_Cases.Queries;
using Application.Utils;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Gridify;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.OutfitQueryHandlers
{
    public class GetFilteredOutfitsQueryHandler : IRequestHandler<GetFilteredQuery<Outfit, OutfitDTO>, Result<PagedResult<OutfitDTO>>>
    {
        private readonly IOutfitRepository outfitRepository;
        private readonly IMapper mapper;

        public GetFilteredOutfitsQueryHandler(IOutfitRepository outfitRepository, IMapper mapper)
        {
            this.outfitRepository = outfitRepository;
            this.mapper = mapper;
        }

        public async Task<Result<PagedResult<OutfitDTO>>> Handle(GetFilteredQuery<Outfit, OutfitDTO> request, CancellationToken cancellationToken)
        {
            var outfits = await outfitRepository.GetAllAsync();

            if (request.Filter != null)
            {
                outfits = outfits.AsQueryable()
                    .Where(request.Filter);
            }

            var totalCount = outfits.Count();
            var pagedItems = outfits.AsQueryable().ApplyPaging(request.Page, request.PageSize).ToList();

            var outfitDtos = pagedItems.Select(outfit => new OutfitDTO
            {
                Id = outfit.Id,
                UserId = outfit.UserId,
                Name = outfit.Name,
                CreatedAt = outfit.CreatedAt,
                Season = outfit.Season,
                Description = outfit.Description,
                ImageUrl = outfit.ImageUrl,
                ClothingItemDTOs = outfit.OutfitClothingItems
                    .Where(oci => oci.ClothingItem != null)
                    .Select(oci => new ClothingItemDTO
                    {
                        Id = oci.ClothingItem.Id,
                        UserId = oci.ClothingItem.UserId,
                        Name = oci.ClothingItem.Name,
                        Category = oci.ClothingItem.Category,
                        Color = oci.ClothingItem.Color,
                        Brand = oci.ClothingItem.Brand,
                        Material = oci.ClothingItem.Material,
                        PrintType = oci.ClothingItem.PrintType,
                        PrintDescription = oci.ClothingItem.PrintDescription,
                        Description = oci.ClothingItem.Description,
                        FrontImageUrl = oci.ClothingItem.FrontImageUrl,
                        BackImageUrl = oci.ClothingItem.BackImageUrl,
                        NumberOfWears = oci.ClothingItem.NumberOfWears,
                        LastWornDate = oci.ClothingItem.LastWornDate
                    })
                    .ToList()
            }).ToList();

            var pagedResult = new PagedResult<OutfitDTO>(outfitDtos, totalCount);

            return Result<PagedResult<OutfitDTO>>.Success(pagedResult);
        }

    }
}
