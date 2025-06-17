using Application.DTOs;
using Application.Services;
using Application.Use_Cases.CommandHandlers.OutfitCommandHandlers;
using Application.Use_Cases.Commands.OutfitCommands;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReWear.Application.UnitTests.OutfitUnitTests
{
    public class MatchOutfitItemsCommandHandlerTests
    {
        private readonly IOutfitRepository outfitRepository;
        private readonly IClothingItemRepository clothingItemRepository;
        private readonly IEmbeddingService embeddingService;
        private readonly IMapper mapper;
        private readonly MatchOutfitItemsCommandHandler handler;

        public MatchOutfitItemsCommandHandlerTests()
        {
            outfitRepository = Substitute.For<IOutfitRepository>();
            clothingItemRepository = Substitute.For<IClothingItemRepository>();
            embeddingService = Substitute.For<IEmbeddingService>();
            mapper = Substitute.For<IMapper>();
            handler = new MatchOutfitItemsCommandHandler(outfitRepository, clothingItemRepository, embeddingService, mapper);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenDescriptionIsNullOrEmpty()
        {
            var command = new MatchOutfitItemsCommand
            {
                UserId = Guid.Parse("8c1ae239-734b-4d8d-891d-5e7fd40ea662"),
                Description = null
            };

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Description cannot be null or empty.");
        }

        [Fact]
        public async Task Handle_ShouldReturnTop3SimilarItems_WhenItemsExist()
        {
            // Arrange
            var userId = Guid.Parse("8c1ae239-734b-4d8d-891d-5e7fd40ea662");
            var description = "A summer outfit";
            var outfitEmbedding = new float[] { 1, 0, 0 };

            var items = new List<ClothingItem>
            {
                CreateClothingItem(userId, new float[] { 1, 0, 0 }),
                CreateClothingItem(userId, new float[] { 0.7f, 0.7f, 0 }),
                CreateClothingItem(userId, new float[] { 0.7f, 0, 0.7f }),
                CreateClothingItem(userId, new float[] { 0, 1, 0 }), // below threshold
                CreateClothingItem(Guid.NewGuid(), new float[] { 1, 0, 0 }) // different user
            };

            clothingItemRepository.GetAllAsync().Returns(items);
            embeddingService.GetEmbeddingAsync(description).Returns(Task.FromResult(outfitEmbedding));
            embeddingService.ComputeCosineSimilarity(outfitEmbedding, items[0].Embedding).Returns(1.0f);
            embeddingService.ComputeCosineSimilarity(outfitEmbedding, items[1].Embedding).Returns(0.7f);
            embeddingService.ComputeCosineSimilarity(outfitEmbedding, items[2].Embedding).Returns(0.7f);
            embeddingService.ComputeCosineSimilarity(outfitEmbedding, items[3].Embedding).Returns(0.0f);
            embeddingService.ComputeCosineSimilarity(outfitEmbedding, items[4].Embedding).Returns(1.0f);

            var dtos = new List<ClothingItemDTO>
            {
                CreateClothingItemDTO(items[0]),
                CreateClothingItemDTO(items[1]),
                CreateClothingItemDTO(items[2])
            };
            mapper.Map<List<ClothingItemDTO>>(Arg.Any<List<ClothingItem>>()).Returns(dtos);

            var command = new MatchOutfitItemsCommand
            {
                UserId = userId,
                Description = description
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(dtos);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoSimilarItemsFound()
        {
            // Arrange
            var userId = Guid.Parse("8c1ae239-734b-4d8d-891d-5e7fd40ea662");
            var description = "A winter outfit";
            var outfitEmbedding = new float[] { 0, 1, 0 };

            var items = new List<ClothingItem>
            {
                CreateClothingItem(userId, new float[] { 1, 0, 0 }),
                CreateClothingItem(userId, new float[] { 1, 0, 0 })
            };

            clothingItemRepository.GetAllAsync().Returns(items);
            embeddingService.GetEmbeddingAsync(description).Returns(Task.FromResult(outfitEmbedding));
            embeddingService.ComputeCosineSimilarity(outfitEmbedding, items[0].Embedding).Returns(0.2f);
            embeddingService.ComputeCosineSimilarity(outfitEmbedding, items[1].Embedding).Returns(0.3f);

            mapper.Map<List<ClothingItemDTO>>(Arg.Any<List<ClothingItem>>()).Returns(new List<ClothingItemDTO>());

            var command = new MatchOutfitItemsCommand
            {
                UserId = userId,
                Description = description
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEmpty();
        }
        private ClothingItem CreateClothingItem(Guid userId, float[] embedding)
        {
            return new ClothingItem
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Shirt",
                Category = "Top",
                Tags = new List<Domain.Models.ClothingTag> { new Domain.Models.ClothingTag { Tag = "casual" } },
                Color = "Blue",
                Brand = "BrandX",
                Material = "Cotton",
                PrintType = "None",
                PrintDescription = "No print",
                Description = "A blue shirt",
                FrontImageUrl = "front.jpg",
                BackImageUrl = "back.jpg",
                Embedding = embedding,
                CreatedAt = DateTime.UtcNow,
                NumberOfWears = 5,
                LastWornDate = DateTime.UtcNow.AddDays(-1),
                Weight = 0.5m,
                IsSold = false,
                OutfitClothingItems = new List<OutfitClothingItem>()
            };
        }

        private ClothingItemDTO CreateClothingItemDTO(ClothingItem item)
        {
            return new ClothingItemDTO
            {
                Id = item.Id,
                UserId = item.UserId,
                Name = item.Name,
                Category = item.Category,
                Tags = item.Tags?.Select(t => t.Tag).ToList() ?? new List<string>(),
                Color = item.Color,
                Brand = item.Brand,
                Material = item.Material,
                PrintType = item.PrintType,
                PrintDescription = item.PrintDescription,
                Description = item.Description,
                FrontImageUrl = item.FrontImageUrl,
                BackImageUrl = item.BackImageUrl,
                NumberOfWears = (uint?)item.NumberOfWears,
                LastWornDate = item.LastWornDate,
            };
        }
    }
}