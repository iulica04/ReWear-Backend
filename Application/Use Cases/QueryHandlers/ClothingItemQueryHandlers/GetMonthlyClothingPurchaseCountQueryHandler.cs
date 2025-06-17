using Application.Use_Cases.Queries.ClothingItemQueries;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.ClothingItemQueryHandlers
{
    public class GetMonthlyClothingPurchaseCountQueryHandler : IRequestHandler<GetMonthlyClothingPurchaseCountQuery, Dictionary<string, int>>
    {
        private readonly IClothingItemRepository repository;
        public GetMonthlyClothingPurchaseCountQueryHandler(IClothingItemRepository repository)
        {
            this.repository = repository;
        }
        public async Task<Dictionary<string, int>> Handle(GetMonthlyClothingPurchaseCountQuery request, CancellationToken cancellationToken)
        {
            return await repository.GetMonthlyPurchaseCountAsync(request.UserId);
        }
    }
}
