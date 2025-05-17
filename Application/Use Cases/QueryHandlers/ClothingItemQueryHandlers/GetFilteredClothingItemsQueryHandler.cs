using Application.DTOs;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;
using Gridify;
using Application.Utils;

namespace Application.Use_Cases.Queries.ClothingItemQueries
{
    public class GetFilteredClothingItemsQueryHandler : IRequestHandler<GetFilteredQuery<ClothingItem, ClothingItemDTO>, Result<PagedResult<ClothingItemDTO>>>
    {
        private readonly IClothingItemRepository repository;
        private readonly IMapper mapper;
        public GetFilteredClothingItemsQueryHandler(IClothingItemRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<PagedResult<ClothingItemDTO>>> Handle(GetFilteredQuery<ClothingItem, ClothingItemDTO> request, CancellationToken cancellationToken)
        {
            var clothingItems = await repository.GetAllAsync();
            if(request.Filter != null)
            {
                clothingItems = clothingItems.AsQueryable()
                    .Where(request.Filter);
            }

            var totalCount = clothingItems.Count();
            var pagedItems = clothingItems.AsQueryable().ApplyPaging(request.Page, request.PageSize);

            var clothingItemDtos = mapper.Map<List<ClothingItemDTO>>(pagedItems.ToList());
            var pagedResult = new PagedResult<ClothingItemDTO>(clothingItemDtos, totalCount);

            return Result<PagedResult<ClothingItemDTO>>.Success(pagedResult);
        }
    }
}
