using Application.DTOs;
using Application.Use_Cases.Queries.ClothingItemQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.ClothingItemQueryHandlers
{
    public class GetClothingItemsByNameQueryHandler : IRequestHandler<GetClothingItemsByNameQuery, Result<List<ClothingItemDTO>>>
    {
        private readonly IClothingItemRepository clothingItemRepository;
        private readonly IMapper mapper;

        public GetClothingItemsByNameQueryHandler(IClothingItemRepository clothingItemRepository, IMapper mapper)
        {
            this.clothingItemRepository = clothingItemRepository;
            this.mapper = mapper;
        }

        public async Task<Result<List<ClothingItemDTO>>> Handle(GetClothingItemsByNameQuery request, CancellationToken cancellationToken)
        {
            var allItems = await clothingItemRepository.GetAllAsync();
            var filtered = allItems
                .Where(x => x.UserId == request.UserId &&
                            !string.IsNullOrEmpty(x.Name) &&
                            x.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var dtos = mapper.Map<List<ClothingItemDTO>>(filtered);
            return Result<List<ClothingItemDTO>>.Success(dtos);
        }
    }
}
