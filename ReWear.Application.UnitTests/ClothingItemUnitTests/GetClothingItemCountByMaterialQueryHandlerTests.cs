using Application.Use_Cases.Queries.ClothingItemQueries;
using Application.Use_Cases.QueryHandlers.ClothingItemQueryHandlers;
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
    public class GetClothingItemCountByMaterialQueryHandlerTests
    {
        private readonly IClothingItemRepository clothingItemRepository;

        public GetClothingItemCountByMaterialQueryHandlerTests()
        {
            this.clothingItemRepository = Substitute.For<IClothingItemRepository>();
        }

        [Fact]
        public async Task Given_GetClothingItemCountByMaterialQueryHandler_When_RepositoryReturnsData_Then_ReturnsCorrectDictionary()
        {
            // Arrange
            var userId = Guid.Parse("9c922454-33a3-498f-ad9d-d62173cd3bef");
            var expectedCounts = new Dictionary<string, int>
            {
                { "Cotton", 3 },
                { "Wool", 2 },
                { "Polyester", 5 }
            };

            clothingItemRepository.GetCountByMaterialAsync(userId).Returns(expectedCounts);

            var query = new GetClothingItemCountByMaterialQuery { UserId = userId };
            var handler = new GetClothingItemCountByMaterialQueryHandler(clothingItemRepository);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result["Cotton"].Should().Be(3);
            result["Wool"].Should().Be(2);
            result["Polyester"].Should().Be(5);
        }

        [Fact]
        public async Task Given_GetClothingItemCountByMaterialQueryHandler_When_NoDataExists_Then_ReturnsEmptyDictionary()
        {
            // Arrange
            var userId = Guid.Parse("9c922454-33a3-498f-ad9d-d62173cd3bef");
            clothingItemRepository.GetCountByMaterialAsync(userId).Returns(new Dictionary<string, int>());

            var query = new GetClothingItemCountByMaterialQuery { UserId = userId };
            var handler = new GetClothingItemCountByMaterialQueryHandler(clothingItemRepository);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}
