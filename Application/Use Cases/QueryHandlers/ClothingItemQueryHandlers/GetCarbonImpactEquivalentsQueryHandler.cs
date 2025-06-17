using Application.Use_Cases.Queries.ClothingItemQueries;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.ClothingItemQueryHandlers
{
    public class GetCarbonImpactEquivalentsQueryHandler : IRequestHandler<GetCarbonImpactEquivalentsQuery, Dictionary<string, decimal>>
    {
        public Task<Dictionary<string, decimal>> Handle(GetCarbonImpactEquivalentsQuery request, CancellationToken cancellationToken)
        {
            decimal carbon = request.TotalCarbonImpact;

            var equivalents = new Dictionary<string, decimal>
            {
                { "Kilometers driven by a gasoline car", carbon * 4.6m },
                { "Liters of gasoline consumed", carbon * 0.113m },
                { "Hours of flight (commercial airplane)", carbon * 0.05m },
                { "Kilograms of coal burned", carbon * 0.5m },
                { "Trees planted to offset yearly", carbon * 0.1m },
                { "60W light bulb usage hours", carbon * 8.3m },
                { "Laptop usage hours", carbon * 40m },
                { "Cups of coffee consumed", carbon * 2.5m },
                { "Kilometers traveled by electric train", carbon * 12m },
                { "Minutes of HD video streaming", carbon * 30m },
                { "Smartphone charges", carbon * 1220m },
                { "Washing machine cycles", carbon * 6.3m },
                { "Showers (10 minutes)", carbon * 5.0m },
                { "Plastic bags produced", carbon * 80m },
                { "Plastic bottles produced (500ml)", carbon * 30m },
                { "Emails sent (with attachment)", carbon * 400m },
                { "Reams of paper produced", carbon * 0.02m },
                { "Hours using a microwave oven", carbon * 2m },
                { "Light meals prepared with a gas stove", carbon * 3.5m },
                { "Dishwasher cycles", carbon * 2.1m }
            };

            return Task.FromResult(equivalents);
        }
    }
}
