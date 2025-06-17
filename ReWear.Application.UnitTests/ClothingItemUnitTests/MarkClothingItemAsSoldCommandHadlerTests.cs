using Application.Use_Cases.CommandHandlers.ClothingItemCommandHandlers;
using Application.Use_Cases.Commands.ClothingItemCommands;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace ReWear.Application.UnitTests.ClothingItemUnitTests
{
    public class MarkClothingItemAsSoldCommandHandlerTests
    {
        private readonly IClothingItemRepository repository;
        private readonly MarkClothingItemAsSoldCommandHadler handler;

        public MarkClothingItemAsSoldCommandHandlerTests()
        {
            repository = Substitute.For<IClothingItemRepository>();
            handler = new MarkClothingItemAsSoldCommandHadler(repository);
        }

        [Fact]
        public async Task Handle_ShouldMarkClothingItemAsSold_WhenItemExists()
        {
            // Arrange
            var itemId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

            var clothingItem = new ClothingItem
            {
                Id = itemId,
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Name = "Shirt",
                Category = "Top",
                Color = "Blue",
                Brand = "BrandX",
                Material = "Cotton",
                FrontImageUrl = "front.jpg",
                CreatedAt = DateTime.UtcNow,
                Tags = new(),
                OutfitClothingItems = new(),
                Weight = 0.2m,
                IsSold = false
            };

            repository.GetByIdAsync(itemId).Returns(clothingItem);

            var command = new MarkClothingItemAsSoldCommand {Id = itemId };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            clothingItem.IsSold.Should().BeTrue();
            await repository.Received(1).UpdateAsync(clothingItem);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenItemNotFound()
        {
            // Arrange
            var nonExistentId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
            repository.GetByIdAsync(nonExistentId).Returns((ClothingItem?)null);

            var command = new MarkClothingItemAsSoldCommand { Id = nonExistentId };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Clothing item not found");
            await repository.DidNotReceive().UpdateAsync(Arg.Any<ClothingItem>());
        }
    }
}
