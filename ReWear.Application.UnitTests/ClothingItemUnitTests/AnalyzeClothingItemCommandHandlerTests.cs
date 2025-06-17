using Application.Models;
using Application.Services;
using Application.Use_Cases.CommandHandlers.ClothingItemCommandHandlers;
using Application.Use_Cases.Commands.ClothingItemCommand;
using Domain.Common;
using FluentAssertions;
using NSubstitute;

namespace ReWear.Application.UnitTests.ClothingItemUnitTests
{
    public class AnalyzeClothingItemCommandHandlerTests
    {
        private readonly IClothingItemService clothingItemService;
        private readonly AnalyzeClothingItemCommandHandler handler;

        public AnalyzeClothingItemCommandHandlerTests()
        {
            clothingItemService = Substitute.For<IClothingItemService>();
            handler = new AnalyzeClothingItemCommandHandler(clothingItemService);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenServiceReturnsSuccess()
        {
            // Arrange
            var imageFront = new byte[] { 1, 2, 3 };
            var imageBack = new byte[] { 4, 5, 6 };
            var command = new AnalyzeClothingItemCommand
            {
                ImageFront = imageFront,
                ImageBack = imageBack
            };

            var expectedResult = new ImageAnalysisResult
            {
                Name = "Classic Shirt",
                Category = "Shirt",
                Tags = new() { "Casual", "Summer" },
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                Weight = 0.25m
            };

            clothingItemService.AnalyzeClothingItemsAsync(imageFront, imageBack)
                .Returns(Task.FromResult(Result<ImageAnalysisResult>.Success(expectedResult)));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Name.Should().Be(expectedResult.Name);
            result.Data.Category.Should().Be(expectedResult.Category);
            result.Data.Tags.Should().BeEquivalentTo(expectedResult.Tags);
            result.Data.Color.Should().Be(expectedResult.Color);
            result.Data.Brand.Should().Be(expectedResult.Brand);
            result.Data.Material.Should().Be(expectedResult.Material);
            result.Data.Weight.Should().Be(expectedResult.Weight);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenServiceReturnsFailure()
        {
            // Arrange
            var command = new AnalyzeClothingItemCommand
            {
                ImageFront = new byte[] { 1, 2, 3 },
                ImageBack = null
            };

            var errorMessage = "Image analysis failed";

            clothingItemService.AnalyzeClothingItemsAsync(command.ImageFront, command.ImageBack)
                .Returns(Task.FromResult(Result<ImageAnalysisResult>.Failure(errorMessage)));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be(errorMessage);
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenImageFrontIsEmpty()
        {
            // Arrange
            var command = new AnalyzeClothingItemCommand
            {
                ImageFront = new byte[0],  // empty array
                ImageBack = null
            };

            var errorMessage = "Front image data cannot be empty.";

  
            clothingItemService.AnalyzeClothingItemsAsync(command.ImageFront, command.ImageBack)
                .Returns(Task.FromResult(Result<ImageAnalysisResult>.Failure(errorMessage)));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be(errorMessage);
            result.Data.Should().BeNull();
        }

    }
}
