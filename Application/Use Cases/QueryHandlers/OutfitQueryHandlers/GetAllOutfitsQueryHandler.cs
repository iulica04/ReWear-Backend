using Application.DTOs;
using Application.Use_Cases.Queries.OutfitQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.OutfitQueryHandlers
{
    public class GetAllOutfitsQueryHandler : IRequestHandler<GetAllOutfitsQuery, Result<List<OutfitDTO>>>
    {
        private readonly IOutfitRepository repository;
        private readonly IMapper mapper;

        public GetAllOutfitsQueryHandler(IOutfitRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<Result<List<OutfitDTO>>> Handle(GetAllOutfitsQuery request, CancellationToken cancellationToken)
        {
            var outfits = await repository.GetAllAsync();

            var outfitDtos = outfits.Select(outfit => new OutfitDTO
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
                     //   Tags = oci.ClothingItem.Tags,
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
                    }).ToList()
            }).ToList();

            return Result<List<OutfitDTO>>.Success(outfitDtos);
        }

    }
}
