using Application.Use_Cases.Queries.ClothingItemQueries;
using Application.Use_Cases.QueryHandlers.ClothingItemQueryHandlers;
using Domain.Entities;
using Domain.Models;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace ReWear.Application.UnitTests.ClothingItemUnitTests
{
    public class GetClothingItemsAvaibleForSaleQueryHandlerTests
    {
        private readonly IClothingItemRepository clothingItemRepository;

        public GetClothingItemsAvaibleForSaleQueryHandlerTests()
        {
            this.clothingItemRepository = Substitute.For<IClothingItemRepository>();
        }

        [Fact]
        public async Task Given_Handler_When_ItemsAvailable_Then_ReturnsSuccessWithItems()
        {
            // Arrange
            var userId = Guid.Parse("9c922454-33a3-498f-ad9d-d62173cd3bef");
            var clothingItems = new List<ClothingItem>
            {
                new ClothingItem
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "Jacket",
                    Category = "Outerwear",
                    Tags = new List<ClothingTag> { new ClothingTag { Tag = "Winter" }, new ClothingTag { Tag = "Casual" } },
                    Color = "Blue",
                    Brand = "Zara",
                    Material = "Cotton",
                    Description = "Warm and stylish",
                    PrintDescription = "None",
                    PrintType = "None",
                    FrontImageUrl = "front.jpg",
                    BackImageUrl = "back.jpg",
                    NumberOfWears = 1,
                    LastWornDate = DateTime.Now.AddMonths(-7)
                }
            };

            clothingItemRepository.GetUnusedInLastSixMonthsAsync(userId).Returns(clothingItems);

            var query = new GetClothingItemsAvaibleForSaleQuery { UserId = userId };
            var handler = new GetClothingItemsAvaibleForSaleQueryHandler(clothingItemRepository);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().HaveCount(1);
            result.Data.First().Name.Should().Be("Jacket");
            result.Data.First().Tags.Should().Contain(new[] { "Winter", "Casual" });
        }

        [Fact]
        public async Task Given_Handler_When_NoItemsAvailable_Then_ReturnsFailure()
        {
            // Arrange
            var userId = Guid.Parse("9c922454-33a3-498f-ad9d-d62173cd3bef");
            clothingItemRepository.GetUnusedInLastSixMonthsAsync(userId).Returns(new List<ClothingItem>());

            var query = new GetClothingItemsAvaibleForSaleQuery { UserId = userId };
            var handler = new GetClothingItemsAvaibleForSaleQueryHandler(clothingItemRepository);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("No clothing items available for sale.");
        }
    }
}
