using Application.Models;
using Application.Use_Cases.Commands.ClothingItemCommands;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.ClothingItemCommandHandlers
{
    public class EstimateCarbonFootprintCommandHandler : IRequestHandler<EstimateCarbonFootprintCommand, Result<EstimateCarbonFootprintResult>>
    {
        private readonly IClothingItemRepository repository;
        private readonly Dictionary<string, decimal> carbonEmissionsPerKg =
        new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
        {
            { "cotton", 16.4m },
            { "organic cotton", 2.1m },         // mult mai mică decât bumbacul convențional
            { "polyester", 14.2m },
            { "recycled polyester", 5.5m },
            { "wool", 80.3m },
            { "merino wool", 73.8m },
            { "cashmere wool", 385.5m },
            { "linen", 16.7m },
            { "hemp", 8.3m },
            { "silk", 18.6m },
            { "viscose", 10.1m },
            { "modal", 9.9m },
            { "lyocell", 6.1m },
            { "nylon", 20.0m },
            { "recycled nylon", 6.0m },
            { "acrylic", 21.1m },
            { "elastane", 23.0m },               // cunoscut și ca spandex
            { "rayon", 9.5m },
            { "tweed", 45.0m },                  // estimat ca tip de lână tratată
            { "denim", 20.0m },                  // în funcție de tratament (spălare, vopsire etc.)
            { "fleece", 14.5m },                 // poliester texturat
            { "leather", 110.0m },               // natural tanned bovine leather
            { "recycled cotton", 4.0m },
            { "bamboo viscose", 10.0m },         // tratată chimic
            { "bamboo lyocell", 6.5m }           // tratată ecologic (closed-loop)
        };


        public EstimateCarbonFootprintCommandHandler(IClothingItemRepository repository)
        {
            this.repository = repository;
        }
        public async Task<Result<EstimateCarbonFootprintResult>> Handle(EstimateCarbonFootprintCommand request, CancellationToken cancellationToken)
        {
            var clothingItems = repository.GetAllAsync().Result
                .Where(ci => ci.UserId == request.UserId);

            decimal totalEmissions = 0;
            int count = 0;

            foreach (var item in clothingItems)
            {
                string materialKey = item.Material?.Trim().ToLower() ?? string.Empty;

                var match = carbonEmissionsPerKg
                    .FirstOrDefault(entry => materialKey.Contains(entry.Key.ToLower()));

                if (!string.IsNullOrEmpty(match.Key)) // dacă a găsit o potrivire parțială
                {
                    count++;
                    if (item.Weight.HasValue)
                    {
                        totalEmissions += item.Weight.Value * match.Value;
                    }
                }
            }

            var result = new EstimateCarbonFootprintResult
            {
                TotalCarbonFootprint = totalEmissions,
                CountedItems = count,
                TotalNumberOfItems = clothingItems.Count()
            };

            return Result<EstimateCarbonFootprintResult>.Success(result);
        }
    }
}
