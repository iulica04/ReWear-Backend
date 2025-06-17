using MediatR;

namespace Application.Use_Cases.Queries.ClothingItemQueries
{
    public class GetMonthlyClothingPurchaseCountQuery : IRequest<Dictionary<string, int>>
    {
        public Guid UserId { get; set; }
    }
}
