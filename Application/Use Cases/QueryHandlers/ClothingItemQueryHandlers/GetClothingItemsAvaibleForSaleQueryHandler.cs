

using Application.DTOs;
using Application.Use_Cases.Queries.ClothingItemQueries;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.ClothingItemQueryHandlers
{
    public class GetClothingItemsAvaibleForSaleQueryHandler : IRequestHandler<GetClothingItemsAvaibleForSaleQuery, Result<List<ClothingItemDTO>>>
    {
        private readonly IClothingItemRepository clothingItemRepository;

        public GetClothingItemsAvaibleForSaleQueryHandler(IClothingItemRepository clothingItemRepository)
        {
            this.clothingItemRepository = clothingItemRepository;
        }
        public async Task<Result<List<ClothingItemDTO>>> Handle(GetClothingItemsAvaibleForSaleQuery request, CancellationToken cancellationToken)
        {
            var clothingItems = await clothingItemRepository.GetUnusedInLastSixMonthsAsync(request.UserId);
            if (clothingItems == null || !clothingItems.Any())
            {
                return Result<List<ClothingItemDTO>>.Failure("No clothing items available for sale.");
            }
            var clothingItemDTOs = clothingItems.Select(ci => new ClothingItemDTO
            {
                Id = ci.Id,
                UserId = ci.UserId,
                Name = ci.Name,
                Category = ci.Category,
                Tags = ci.Tags.Select(t => t.Tag).ToList(),
                Color = ci.Color,
                Brand = ci.Brand,
                Material = ci.Material,
                Description = ci.Description,
                PrintDescription = ci.PrintDescription,
                PrintType = ci.PrintType,
                FrontImageUrl = ci.FrontImageUrl,
                BackImageUrl = ci.BackImageUrl,
                NumberOfWears = ci.NumberOfWears,
                LastWornDate = ci.LastWornDate

            }).ToList();
            return Result<List<ClothingItemDTO>>.Success(clothingItemDTOs);
        }
    }
}
