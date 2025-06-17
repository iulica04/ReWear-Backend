using Application.Use_Cases.QueryHandlers.ClothingItemQueryHandlers;
using Application.Use_Cases.Queries.ClothingItemQueries;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReWear.Application.UnitTests.ClothingItemUnitTests
{
    public class GetMonthlyClothingPurchaseCountQueryHandlerTests
    {
        [Fact]
        public async Task Given_ValidUserId_When_HandlerIsCalled_Then_ReturnsMonthlyPurchaseCount()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedResult = new Dictionary<string, int>
            {
                { "2025-01", 5 },
                { "2025-02", 3 },
                { "2025-03", 7 }
            };

            var repository = Substitute.For<IClothingItemRepository>();
            repository.GetMonthlyPurchaseCountAsync(userId).Returns(Task.FromResult(expectedResult));

            var query = new GetMonthlyClothingPurchaseCountQuery {UserId = userId };
            var handler = new GetMonthlyClothingPurchaseCountQueryHandler(repository);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
