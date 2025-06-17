using Application.Use_Cases.CommandHandlers.ClothingItemCommandHandlers;
using Application.Use_Cases.Commands.ClothingItemCommand;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReWear.Application.UnitTests.ClothingItemUnitTests
{
    public class DeleteClothingItemCommandHandlerTests
    {
        private readonly IClothingItemRepository clothingItemRepository;
        private readonly DeleteClothingItemCommandHandler handler;

        public DeleteClothingItemCommandHandlerTests()
        {
            clothingItemRepository = Substitute.For<IClothingItemRepository>();
            handler = new DeleteClothingItemCommandHandler(clothingItemRepository);
        }

        [Fact]
        public async Task Handle_ShouldDeleteClothingItem_WhenItemExists()
        {
            // Arrange
            var clothingItemId = new Guid("9c922454-33a3-498f-ad9d-d62173cd3bef");
            var clothingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = Guid.NewGuid(),
                Name = "Blue Jeans",
                Category = "Pants",
                Color = "Blue",
                Brand = "Levi's",
                Material = "Denim",
                FrontImageUrl = "https://example.com/front.jpg",
                CreatedAt = DateTime.UtcNow
            };

            var command = new DeleteClothingItemCommand(clothingItemId);
            clothingItemRepository.GetByIdAsync(clothingItemId).Returns(clothingItem);
            
            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(Unit.Value);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenItemDoesNotExist()
        {
            // Arrange
            var clothingItemId = new Guid("9c922454-33a3-498f-ad9d-d62173cd3bef");
            var command = new DeleteClothingItemCommand(clothingItemId);

            clothingItemRepository.GetByIdAsync(clothingItemId).Returns((ClothingItem?)null);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Clothing item not found");
        }
    }
}
