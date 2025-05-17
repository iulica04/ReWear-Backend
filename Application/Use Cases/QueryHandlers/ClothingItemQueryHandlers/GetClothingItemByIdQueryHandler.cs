using Application.DTOs;
using Application.Use_Cases.Queries.ClothingItemQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.ClothingItemQueryHandlers
{
    public class GetClothingItemByIdQueryHandler : IRequestHandler<GetClothingItemByIdQuery, Result<ClothingItemDTO>>
    {
        private readonly IClothingItemRepository clothingItemRepository;
        private readonly IMapper mapper;
        public GetClothingItemByIdQueryHandler(IClothingItemRepository clothingItemRepository, IMapper mapper)
        {
            this.clothingItemRepository = clothingItemRepository;
            this.mapper = mapper;
        }
        public async Task<Result<ClothingItemDTO>> Handle(GetClothingItemByIdQuery request, CancellationToken cancellationToken)
        {
            var clothingItem = await clothingItemRepository.GetByIdAsync(request.Id);
            if (clothingItem == null)
            {
                return Result<ClothingItemDTO>.Failure("Clothing item not found");
            }
            var clothingItemDto = mapper.Map<ClothingItemDTO>(clothingItem);
            return Result<ClothingItemDTO>.Success(clothingItemDto);
        }
    }
}
