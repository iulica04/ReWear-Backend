using Application.Services;
using Application.Use_Cases.CommandHandlers.OutfitCommandHandlers;
using Application.Use_Cases.Commands.OutfitCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Models;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReWear.Application.UnitTests.OutfitUnitTests
{
    public class UpdateOutfitCommandHandlerTests
    {
        private readonly IOutfitRepository outfitRepository;
        private readonly IMapper mapper;
        private readonly IOutfitService outfitService;
        private readonly IClothingItemRepository clothingItemRepository;
        private readonly IEmbeddingService embeddingService;
        private readonly UpdateOutfitCommandHandler handler;

        public UpdateOutfitCommandHandlerTests()
        {
            outfitRepository = Substitute.For<IOutfitRepository>();
            mapper = Substitute.For<IMapper>();
            outfitService = Substitute.For<IOutfitService>();
            clothingItemRepository = Substitute.For<IClothingItemRepository>();
            embeddingService = Substitute.For<IEmbeddingService>();
            handler = new UpdateOutfitCommandHandler(outfitRepository, mapper, outfitService, clothingItemRepository, embeddingService);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenOutfitNotFound()
        {
            var command = new UpdateOutfitCommand
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Name = "Test",
                ClothingItemIds = new List<Guid>(),
                ImageFront = null
            };
            outfitRepository.GetByIdAsync(command.Id).Returns((Outfit)null);

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Outfit not found");
        }

        [Fact]
        public async Task Handle_ShouldUpdateOutfit_WithAllFields()
        {
            var outfitId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var clothingItemId = Guid.NewGuid();
            var outfit = new Outfit
            {
                Id = outfitId,
                UserId = userId,
                Name = "OldName",
                Style = "OldStyle",
                OutfitClothingItems = new List<OutfitClothingItem>(),
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                Season = "OldSeason",
                Description = "OldDesc",
                ImageUrl = "oldUrl",
                Embedding = new float[] { 0.1f, 0.2f }
            };

            var clothingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = userId,
                Name = "ItemName",
                Category = "Category",
                Tags = new List<ClothingTag> { new ClothingTag {Tag = "Tag1" } },
                Color = "Red",
                Brand = "Brand",
                Material = "Cotton",
                PrintType = "Type",
                PrintDescription = "Desc",
                Description = "Desc",
                FrontImageUrl = "front.jpg",
                BackImageUrl = "back.jpg",
                Embedding = new float[] { 1, 2 },
                CreatedAt = DateTime.UtcNow,
                NumberOfWears = 0,
                LastWornDate = null,
                Weight = 0.5m,
                IsSold = false,
                OutfitClothingItems = new List<OutfitClothingItem>()
            };

            outfitRepository.GetByIdAsync(outfitId).Returns(outfit);
            clothingItemRepository.GetByIdAsync(clothingItemId).Returns(clothingItem);
            outfitService.UploadImageAsync(Arg.Any<byte[]>(), userId.ToString(), outfitId.ToString(), "Outfit", "Front")
                .Returns(Task.FromResult(Result<string>.Success("newUrl")));
            embeddingService.GetEmbeddingAsync("NewDesc").Returns(Task.FromResult(new float[] { 0.5f, 0.5f }));
            outfitRepository.UpdateAsync(Arg.Any<Outfit>()).Returns(Task.FromResult(Result<string>.Success("Outfit updated successfully")));

            var command = new UpdateOutfitCommand
            {
                Id = outfitId,
                UserId = userId,
                Name = "NewName",
                ClothingItemIds = new List<Guid> { clothingItemId },
                Season = "Summer",
                Description = "NewDesc",
                ImageFront = new byte[] { 1, 2, 3 }
            };

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be("Outfit updated successfully");
            outfit.Name.Should().Be("NewName");
            outfit.Season.Should().Be("Summer");
            outfit.Description.Should().Be("NewDesc");
            outfit.ImageUrl.Should().Be("newUrl");
            outfit.Embedding.Should().BeEquivalentTo(new float[] { 0.5f, 0.5f });
            outfit.OutfitClothingItems.Should().ContainSingle(x => x.ClothingItemId == clothingItemId);
            clothingItem.NumberOfWears.Should().Be(1);
        }

        [Fact]
        public async Task Handle_ShouldKeepOldImageUrl_WhenImageFrontIsNull()
        {
            var outfitId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var outfit = new Outfit
            {
                Id = outfitId,
                UserId = userId,
                Name = "OldName",
                ImageUrl = "oldUrl",
                OutfitClothingItems = new List<OutfitClothingItem>()
            };

            outfitRepository.GetByIdAsync(outfitId).Returns(outfit);
            embeddingService.GetEmbeddingAsync(Arg.Any<string>()).Returns(Task.FromResult(new float[] { 1, 2 }));
            outfitRepository.UpdateAsync(Arg.Any<Outfit>()).Returns(Task.FromResult(Result<string>.Success("Outfit updated successfully")));

            var command = new UpdateOutfitCommand
            {
                Id = outfitId,
                UserId = userId,
                Name = "Name",
                ClothingItemIds = new List<Guid>(),
                Description = "desc",
                ImageFront = null
            };

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            outfit.ImageUrl.Should().Be("oldUrl");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUpdateFails()
        {
            var outfitId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var outfit = new Outfit
            {
                Id = outfitId,
                UserId = userId,
                Name = "OldName",
                ImageUrl = "oldUrl",
                OutfitClothingItems = new List<OutfitClothingItem>()
            };

            outfitRepository.GetByIdAsync(outfitId).Returns(outfit);
            embeddingService.GetEmbeddingAsync(Arg.Any<string>()).Returns(Task.FromResult(new float[] { 1, 2 }));
            outfitRepository.UpdateAsync(Arg.Any<Outfit>()).Returns(Task.FromResult(Result<string>.Failure("db error")));

            var command = new UpdateOutfitCommand
            {
                Id = outfitId,
                UserId = userId,
                Name = "Name",
                ClothingItemIds = new List<Guid>(),
                Description = "desc",
                ImageFront = null
            };

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("db error");
        }

        [Fact]
        public async Task Handle_ShouldClearAndAddClothingItems()
        {
            var outfitId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var oldItemId = Guid.NewGuid();
            var newItemId = Guid.NewGuid();

            var outfit = new Outfit
            {
                Id = outfitId,
                UserId = userId,
                Name = "OldName",
                ImageUrl = "oldUrl",
                OutfitClothingItems = new List<OutfitClothingItem>
                {
                    new OutfitClothingItem { OutfitId = outfitId, ClothingItemId = oldItemId }
                }
            };

            var clothingItem = new ClothingItem
            {
                Id = newItemId,
                UserId = userId,
                Name = "ItemName",
                Category = "Category",
                Tags = new List<ClothingTag> { new ClothingTag { Tag = "Tag1" } },
                Color = "Red",
                Brand = "Brand",
                Material = "Cotton",
                PrintType = "Type",
                PrintDescription = "Desc",
                Description = "Desc",
                FrontImageUrl = "front.jpg",
                BackImageUrl = "back.jpg",
                Embedding = new float[] { 1, 2 },
                CreatedAt = DateTime.UtcNow,
                NumberOfWears = 0,
                LastWornDate = null,
                Weight = 0.5m,
                IsSold = false,
                OutfitClothingItems = new List<OutfitClothingItem>()
            };

            outfitRepository.GetByIdAsync(outfitId).Returns(outfit);
            clothingItemRepository.GetByIdAsync(newItemId).Returns(clothingItem);
            embeddingService.GetEmbeddingAsync(Arg.Any<string>()).Returns(Task.FromResult(new float[] { 1, 2 }));
            outfitRepository.UpdateAsync(Arg.Any<Outfit>()).Returns(Task.FromResult(Result<string>.Success("Outfit updated successfully")));

            var command = new UpdateOutfitCommand
            {
                Id = outfitId,
                UserId = userId,
                Name = "Name",
                ClothingItemIds = new List<Guid> { newItemId },
                Description = "desc",
                ImageFront = null
            };

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            outfit.OutfitClothingItems.Should().ContainSingle(x => x.ClothingItemId == newItemId);
            outfit.OutfitClothingItems.Should().NotContain(x => x.ClothingItemId == oldItemId);
            clothingItem.NumberOfWears.Should().Be(1);
        }
    }
}