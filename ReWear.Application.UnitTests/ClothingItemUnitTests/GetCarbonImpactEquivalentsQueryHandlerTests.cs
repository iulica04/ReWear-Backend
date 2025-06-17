using Application.Use_Cases.QueryHandlers.ClothingItemQueryHandlers;
using Application.Use_Cases.Queries.ClothingItemQueries;
using FluentAssertions;

namespace ReWear.Application.UnitTests.ClothingItemUnitTests
{
    public class GetCarbonImpactEquivalentsQueryHandlerTests
    {
        [Fact]
        public async Task Given_TotalCarbonImpact_When_HandlerIsCalled_Then_ReturnsCorrectEquivalents()
        {
            // Arrange
            var totalCarbonImpact = 10m;
            var query = new GetCarbonImpactEquivalentsQuery { TotalCarbonImpact = totalCarbonImpact };
            var handler = new GetCarbonImpactEquivalentsQueryHandler();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(20);

            result["Kilometers driven by a gasoline car"].Should().Be(totalCarbonImpact * 4.6m);
            result["Liters of gasoline consumed"].Should().Be(totalCarbonImpact * 0.113m);
            result["Smartphone charges"].Should().Be(totalCarbonImpact * 1220m);
            result["Dishwasher cycles"].Should().Be(totalCarbonImpact * 2.1m);
        }
    }
}
