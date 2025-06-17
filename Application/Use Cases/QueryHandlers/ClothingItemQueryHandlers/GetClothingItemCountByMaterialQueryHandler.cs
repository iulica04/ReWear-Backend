using Application.Use_Cases.Queries.ClothingItemQueries;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.ClothingItemQueryHandlers
{
    public class GetClothingItemCountByMaterialQueryHandler : IRequestHandler<GetClothingItemCountByMaterialQuery, Dictionary<string, int>>
    {
        private readonly IClothingItemRepository repository;

        public GetClothingItemCountByMaterialQueryHandler(IClothingItemRepository repository)
        {
            this.repository = repository;
        }
        public async Task<Dictionary<string, int>> Handle(GetClothingItemCountByMaterialQuery request, CancellationToken cancellationToken)
        {
            return await repository.GetCountByMaterialAsync(request.UserId);
        }
    }
}
