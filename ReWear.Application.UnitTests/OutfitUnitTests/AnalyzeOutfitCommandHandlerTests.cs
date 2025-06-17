using Application.Models;
using Application.Services;
using Application.Use_Cases.CommandHandlers.OutfitCommandHandlers;
using Application.Use_Cases.Commands.OutfitCommands;
using Domain.Common;
using Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace ReWear.Application.UnitTests.OutfitUnitTests
{
    public class AnalyzeOutfitCommandHandlerTests
    {
        private readonly IOutfitService outfitService;
        private readonly AnalyzeOutfitCommandHandler handler;

        public AnalyzeOutfitCommandHandlerTests()
        {
            outfitService = Substitute.For<IOutfitService>();
            handler = new AnalyzeOutfitCommandHandler(outfitService);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenServiceReturnsSuccess()
        {
            // Arrange
            var image = new byte[] { 1, 2, 3 };
            var analysisResult = new OutfitAnalysisResult
            {
                Name = "Test",
                Style = "Casual",
                Season = "Summer",
                Description = "A nice outfit"
            };
            var serviceResult = Result<OutfitAnalysisResult>.Success(analysisResult);

            outfitService.AnalyzeOutfitAsync(image).Returns(Task.FromResult(serviceResult));

            var command = new AnalyzeOutfitCommand { ImageFront = image };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(analysisResult);
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenServiceReturnsFailure()
        {
            // Arrange
            var image = new byte[] { 4, 5, 6 };
            var errorMessage = "Analysis failed";
            var serviceResult = Result<OutfitAnalysisResult>.Failure(errorMessage);

            outfitService.AnalyzeOutfitAsync(image).Returns(Task.FromResult(serviceResult));

            var command = new AnalyzeOutfitCommand { ImageFront = image };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.ErrorMessage.Should().Be(errorMessage);
        }

        [Fact]
        public async Task Handle_ShouldCallService_WithGivenImage()
        {
            // Arrange
            var image = new byte[] { 7, 8, 9 };
            var serviceResult = Result<OutfitAnalysisResult>.Success(new OutfitAnalysisResult());
            outfitService.AnalyzeOutfitAsync(image).Returns(Task.FromResult(serviceResult));

            var command = new AnalyzeOutfitCommand { ImageFront = image };

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            await outfitService.Received(1).AnalyzeOutfitAsync(image);
        }

        [Fact]
        public void AnalyzeOutfitCommand_ShouldAssignImageFront()
        {
            // Arrange
            var image = new byte[] { 10, 11 };
            var command = new AnalyzeOutfitCommand { ImageFront = image };

            // Assert
            command.ImageFront.Should().BeSameAs(image);
        }

        [Fact]
        public void OutfitAnalysisResult_ShouldAllowNullProperties()
        {
            // Arrange
            var result = new OutfitAnalysisResult();

            // Assert
            result.Name.Should().BeNull();
            result.Style.Should().BeNull();
            result.Season.Should().BeNull();
            result.Description.Should().BeNull();
        }

        [Fact]
        public void OutfitAnalysisResult_ShouldAssignProperties()
        {
            // Arrange
            var result = new OutfitAnalysisResult
            {
                Name = "Name",
                Style = "Style",
                Season = "Season",
                Description = "Description"
            };

            // Assert
            result.Name.Should().Be("Name");
            result.Style.Should().Be("Style");
            result.Season.Should().Be("Season");
            result.Description.Should().Be("Description");
        }

        [Fact]
        public void Outfit_ShouldAssignAndReturnProperties()
        {
            // Arrange
            var id = Guid.Parse("8c1ae239-734b-4d8d-891d-5e7fd40ea662");
            var userId = Guid.Parse("8c1ae239-734b-4d8d-891d-5e7fd40ea662");
            var name = "OutfitName";
            var style = "Sport";
            var createdAt = DateTime.UtcNow;
            var season = "Winter";
            var description = "Desc";
            var imageUrl = "http://img";
            var embedding = new float[] { 1.1f, 2.2f };
            var clothingItems = new List<OutfitClothingItem>
            {
                new OutfitClothingItem()
            };

            var outfit = new Outfit
            {
                Id = id,
                UserId = userId,
                Name = name,
                Style = style,
                CreatedAt = createdAt,
                Season = season,
                Description = description,
                ImageUrl = imageUrl,
                Embedding = embedding,
                OutfitClothingItems = clothingItems
            };

            // Assert
            outfit.Id.Should().Be(id);
            outfit.UserId.Should().Be(userId);
            outfit.Name.Should().Be(name);
            outfit.Style.Should().Be(style);
            outfit.CreatedAt.Should().BeCloseTo(createdAt, TimeSpan.FromSeconds(1));
            outfit.Season.Should().Be(season);
            outfit.Description.Should().Be(description);
            outfit.ImageUrl.Should().Be(imageUrl);
            outfit.Embedding.Should().BeEquivalentTo(embedding);
            outfit.OutfitClothingItems.Should().BeEquivalentTo(clothingItems);
        }

        [Fact]
        public void Outfit_ShouldHaveDefaultValues()
        {
            // Arrange
            var outfit = new Outfit
            {
                Id = Guid.Parse("8c1ae239-734b-4d8d-891d-5e7fd40ea662"),
                UserId = Guid.Parse("8c1ae239-734b-4d8d-891d-5e7fd40ea662"),
                Name = "Default",
                ImageUrl = "url"
            };

            // Assert
            outfit.Style.Should().BeNull();
            outfit.Season.Should().BeNull();
            outfit.Description.Should().BeNull();
            outfit.Embedding.Should().BeNull();
            outfit.OutfitClothingItems.Should().NotBeNull();
            outfit.OutfitClothingItems.Should().BeEmpty();
        }
    }
}