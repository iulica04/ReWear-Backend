using Google.Apis.Http;
using MediatR;

namespace Application.Use_Cases.Queries.ClothingItemQueries
{
    public class GetCarbonImpactEquivalentsQuery : IRequest<Dictionary<string, decimal>>
    {
        public decimal TotalCarbonImpact { get; set; }
    }
}
