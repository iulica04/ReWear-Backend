using Application.Services;
using Application.Use_Cases.Queries.OutfitQueries;
using Application.Use_Cases.QueryHandlers.OutfitQueryHandlers;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace ReWear.Application.UnitTests.OutfitUnitTests
{
  
    public class GetPaginatedSimilarOutfitsQueryHandlerTests
    {
        private readonly IOutfitRepository repository;
        private readonly IEmbeddingService embeddingService;
        private readonly GetPaginatedSimilarOutfitsQueryHandler handler;
        public GetPaginatedSimilarOutfitsQueryHandlerTests()
        {
            this.repository = Substitute.For<IOutfitRepository>();
            this.embeddingService = Substitute.For<IEmbeddingService>();
            this.handler = new GetPaginatedSimilarOutfitsQueryHandler(repository, embeddingService);
        }
        [Fact]
        public async Task Handle_ShouldReturnPaginatedSimilarOutfits()
        {
            // Arrange
            var targetOutfitId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var targetEmbedding = new float[] { 0.1f, 0.2f, 0.3f };
            var similarEmbedding = new float[] { 0.1f, 0.2f, 0.3f };

            var targetOutfit = new Outfit
            {
                Id = targetOutfitId,
                UserId = userId,
                Name = "Target Outfit",
                CreatedAt = DateTime.UtcNow,
                ImageUrl = "url_target.jpg",
                Embedding = targetEmbedding,
                OutfitClothingItems = new List<OutfitClothingItem>()
            };

            var similarOutfit = new Outfit
            {
                Id = Guid.Parse("f3f8a848-15c7-4cd2-89dd-c8c545871ac0"),
                UserId = userId,
                Name = "Similar Outfit",
                CreatedAt = DateTime.UtcNow,
                ImageUrl = "url_similar.jpg",
                Embedding = similarEmbedding,
                OutfitClothingItems = new List<OutfitClothingItem>
                {
                    new OutfitClothingItem
                    {
                        ClothingItem = new ClothingItem
                        {
                            Id = Guid.Parse("f3f8a848-15c7-4cd2-89dd-c8c545871ac0"),
                            UserId = userId,
                            Name = "Shirt",
                            Category = "Top",
                            Color = "Blue",
                            Brand = "Zara",
                            Material = "Cotton",
                            PrintType = "Striped",
                            PrintDescription = "White stripes",
                            Description = "A blue striped shirt",
                            FrontImageUrl = "front.jpg",
                            BackImageUrl = "back.jpg",
                            NumberOfWears = 5,
                            LastWornDate = DateTime.UtcNow.AddDays(-2)
                        }
                    }
                }
            };

            var otherOutfit = new Outfit
            {
                Id = Guid.Parse("bcac92b0-6796-400b-a494-72fb696d34cf"),
                UserId = userId,
                Name = "Other Outfit",
                CreatedAt = DateTime.UtcNow,
                ImageUrl = "url_other.jpg",
                Embedding = new float[] { 0.9f, 0.9f, 0.9f },
                OutfitClothingItems = new List<OutfitClothingItem>()
            };

            var outfits = new List<Outfit> { targetOutfit, similarOutfit, otherOutfit };

            
            repository.GetAllAsync().Returns(outfits);
            repository.GetByIdAsync(targetOutfitId).Returns(targetOutfit);
            embeddingService
                .ComputeCosineSimilarity(targetEmbedding, similarEmbedding)
                .Returns(0.95f);

            embeddingService
                .ComputeCosineSimilarity(targetEmbedding, otherOutfit.Embedding!)
                .Returns(0.5f);


            var query = new GetPaginatedSimilarOutfitsQuery
            {
                Id = targetOutfitId,
                Page = 1,
                PageSize = 10
            };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.TotalCount.Should().Be(1);

            var dto = result.Data.Data.First();
            dto.Name.Should().Be("Similar Outfit");
            dto.ClothingItemDTOs.Should().HaveCount(1);
            dto.ClothingItemDTOs.First().Name.Should().Be("Shirt");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenTargetOutfitNotFound()
        {
            // Arrange
            var query = new GetPaginatedSimilarOutfitsQuery
            {
                Id = Guid.Parse("f3f8a848-15c7-4cd2-89dd-c8c545871ac0"),
                Page = 1,
                PageSize = 10
            };

            
            repository.GetByIdAsync(query.Id).Returns((Outfit?)null);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Outfit not found");
        }
    }
}