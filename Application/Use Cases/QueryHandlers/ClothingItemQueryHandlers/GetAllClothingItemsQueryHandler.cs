using Application.DTOs;
using Application.Use_Cases.Queries.ClothingItemQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.ClothingItemQueryHandlers
{
    public class GetAllClothingItemsQueryHandler : IRequestHandler<GetAllClothingItemsQuery, Result<List<ClothingItemDTO>>>
    {
        private readonly IClothingItemRepository repository;
        private readonly IMapper mapper;

        public GetAllClothingItemsQueryHandler(IClothingItemRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<Result<List<ClothingItemDTO>>> Handle(GetAllClothingItemsQuery request, CancellationToken cancellationToken)
        {
            var clothingItems = await repository.GetAllAsync();
            return Result<List<ClothingItemDTO>>.Success(mapper.Map<List<ClothingItemDTO>>(clothingItems));
        }
    }
}
