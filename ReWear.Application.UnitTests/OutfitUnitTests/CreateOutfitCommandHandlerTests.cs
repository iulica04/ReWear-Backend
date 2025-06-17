using Application.Services;
using Application.Use_Cases.CommandHandlers.OutfitCommandHandlers;
using Application.Use_Cases.Commands.OutfitCommands;
using Domain.Common;
using Domain.Entities;
using Domain.Models;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ReWear.Application.UnitTests.OutfitUnitTests
{
    public class CreateOutfitCommandHandlerTests
    {
        private readonly IOutfitRepository outfitRepository;
        private readonly IClothingItemRepository clothingItemRepository;
        private readonly IOutfitService outfitService;
        private readonly IEmbeddingService embeddingService;
        private readonly CreateOutfitCommandHandler handler;

        public CreateOutfitCommandHandlerTests()
        {
            outfitRepository = Substitute.For<IOutfitRepository>();
            clothingItemRepository = Substitute.For<IClothingItemRepository>();
            outfitService = Substitute.For<IOutfitService>();
            embeddingService = Substitute.For<IEmbeddingService>();
            handler = new CreateOutfitCommandHandler(outfitRepository, clothingItemRepository, outfitService, embeddingService);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenOutfitIsCreatedWithAllValidItems()
        {
            // Arrange
            var userId = Guid.Parse("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d");
            var clothingItem1Id = Guid.Parse("2a3b4c5d-6e7f-8a9b-0c1d-2e3f4a5b6c7d");
            var clothingItem2Id = Guid.Parse("3a4b5c6d-7e8f-9a0b-1c2d-3e4f5a6b7c8d");

            var imageData = new byte[] { 1, 2, 3, 4, 5 };
            var imageUrl = "https://storage.example.com/outfits/1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d/outfit_front.jpg";
            var embedding = new float[] { 0.1f, 0.2f, 0.3f };

            var clothingItem1 = new ClothingItem
            {
                Id = clothingItem1Id,
                UserId = userId,
                Name = "Shirt",
                Category = "Tops",
                Color = "Blue",
                Brand = "TestBrand",
                Material = "Cotton",
                FrontImageUrl = "https://example.com/image1.jpg",
                Tags = new List<ClothingTag>(),
                NumberOfWears = 0,
                CreatedAt = DateTime.UtcNow,
                LastWornDate = null,
                Weight = 0.5m,
                IsSold = false,
                PrintType = null,
                PrintDescription = null,
                Description = "Test item 1",
                BackImageUrl = null,
                Embedding = null,
                OutfitClothingItems = new List<OutfitClothingItem>()
            };

            var clothingItem2 = new ClothingItem
            {
                Id = clothingItem2Id,
                UserId = userId,
                Name = "Pants",
                Category = "Bottoms",
                Color = "Black",
                Brand = "TestBrand",
                Material = "Denim",
                FrontImageUrl = "https://example.com/image2.jpg",
                Tags = new List<ClothingTag>(),
                NumberOfWears = 0,
                CreatedAt = DateTime.UtcNow,
                LastWornDate = null,
                Weight = 0.8m,
                IsSold = false,
                PrintType = null,
                PrintDescription = null,
                Description = "Test item 2",
                BackImageUrl = null,
                Embedding = null,
                OutfitClothingItems = new List<OutfitClothingItem>()
            };

            var command = new CreateOutfitCommand
            {
                UserId = userId,
                Name = "Summer Casual",
                Style = "Casual",
                ClothingItemIds = new List<Guid> { clothingItem1Id, clothingItem2Id },
                Season = "Summer",
                Description = "Perfect for beach days",
                ImageFront = imageData
            };

            // Set up repository and service mocks
            outfitService.UploadImageAsync(
                imageData,
                userId.ToString(),
                Arg.Any<string>(),
                "Outfit",
                "Front")
                .Returns(Result<string>.Success(imageUrl));

            clothingItemRepository.GetByIdAsync(clothingItem1Id).Returns(clothingItem1);
            clothingItemRepository.GetByIdAsync(clothingItem2Id).Returns(clothingItem2);

            embeddingService.GetEmbeddingAsync("Perfect for beach days").Returns(embedding);

            Outfit capturedOutfit = null;
            outfitRepository.AddAsync(Arg.Do<Outfit>(o => capturedOutfit = o))
                .Returns(Result<Guid>.Success(Guid.NewGuid()));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeEmpty();

            // Verify outfit properties using captured outfit
            capturedOutfit.Should().NotBeNull();
            capturedOutfit.UserId.Should().Be(userId);
            capturedOutfit.Name.Should().Be("Summer Casual");
       //     capturedOutfit.Style.Should().Be("Casual");
            capturedOutfit.Season.Should().Be("Summer");
            capturedOutfit.Description.Should().Be("Perfect for beach days");
            capturedOutfit.ImageUrl.Should().Be(imageUrl);
            capturedOutfit.OutfitClothingItems.Count.Should().Be(2);

            // Verify clothing items were updated
            await clothingItemRepository.Received(1).UpdateAsync(Arg.Is<ClothingItem>(c =>
                c.Id == clothingItem1Id && c.NumberOfWears == 1 && c.LastWornDate != null
            ));
            await clothingItemRepository.Received(1).UpdateAsync(Arg.Is<ClothingItem>(c =>
                c.Id == clothingItem2Id && c.NumberOfWears == 1 && c.LastWornDate != null
            ));
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenOutfitHasNoDescription()
        {
            // Arrange
            var userId = Guid.Parse("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d");
            var clothingItemId = Guid.Parse("2a3b4c5d-6e7f-8a9b-0c1d-2e3f4a5b6c7d");

            var imageData = new byte[] { 1, 2, 3, 4, 5 };
            var imageUrl = "https://storage.example.com/outfits/image.jpg";
            var embedding = new float[] { 0.1f, 0.2f, 0.3f };

            var clothingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = userId,
                Name = "Sweater",
                Category = "Tops",
                Color = "Red",
                Brand = "TestBrand",
                Material = "Wool",
                FrontImageUrl = "https://example.com/sweater.jpg",
                Tags = new List<ClothingTag>(),
                NumberOfWears = 0,
                CreatedAt = DateTime.UtcNow,
                LastWornDate = null,
                Weight = 0.7m,
                IsSold = false,
                PrintType = null,
                PrintDescription = null,
                Description = null,
                BackImageUrl = null,
                Embedding = null,
                OutfitClothingItems = new List<OutfitClothingItem>()
            };

            var command = new CreateOutfitCommand
            {
                UserId = userId,
                Name = "Winter Outfit",
                Style = "Casual",
                ClothingItemIds = new List<Guid> { clothingItemId },
                Season = "Winter",
                Description = null, // No description
                ImageFront = imageData
            };

            // Set up repository and service mocks
            outfitService.UploadImageAsync(
                imageData,
                userId.ToString(),
                Arg.Any<string>(),
                "Outfit",
                "Front")
                .Returns(Result<string>.Success(imageUrl));

            clothingItemRepository.GetByIdAsync(clothingItemId).Returns(clothingItem);

            embeddingService.GetEmbeddingAsync(string.Empty).Returns(embedding);

            outfitRepository.AddAsync(Arg.Any<Outfit>())
                .Returns(callInfo =>
                {
                    var outfit = callInfo.Arg<Outfit>();
                    return Result<Guid>.Success(outfit.Id);
                });

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeEmpty();

            // Verify outfit was created with empty description
            await outfitRepository.Received(1).AddAsync(Arg.Is<Outfit>(o =>
                o.Description == string.Empty
            ));

            // Verify embedding service was called with empty string
            await embeddingService.Received(1).GetEmbeddingAsync(string.Empty);
        }

        [Fact]
        public async Task Handle_ShouldSkipNonExistentClothingItems()
        {
            // Arrange
            var userId = Guid.Parse("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d");
            var existingItemId = Guid.Parse("2a3b4c5d-6e7f-8a9b-0c1d-2e3f4a5b6c7d");
            var nonExistentItemId = Guid.Parse("aaaa0000-0000-0000-0000-000000000000");

            var imageData = new byte[] { 1, 2, 3, 4, 5 };
            var imageUrl = "https://storage.example.com/outfits/image.jpg";
            var embedding = new float[] { 0.1f, 0.2f, 0.3f };

            var existingItem = new ClothingItem
            {
                Id = existingItemId,
                UserId = userId,
                Name = "T-Shirt",
                Category = "Tops",
                Color = "Green",
                Brand = "TestBrand",
                Material = "Cotton",
                FrontImageUrl = "https://example.com/tshirt.jpg",
                Tags = new List<ClothingTag>(),
                NumberOfWears = 0,
                CreatedAt = DateTime.UtcNow,
                LastWornDate = null,
                Weight = 0.4m,
                IsSold = false,
                PrintType = "Graphic",
                PrintDescription = "Logo",
                Description = "Casual t-shirt",
                BackImageUrl = null,
                Embedding = null,
                OutfitClothingItems = new List<OutfitClothingItem>()
            };

            var command = new CreateOutfitCommand
            {
                UserId = userId,
                Name = "Mixed Outfit",
                Style = "Casual",
                ClothingItemIds = new List<Guid> { existingItemId, nonExistentItemId },
                Season = "All",
                Description = "Test outfit",
                ImageFront = imageData
            };

            // Set up repository and service mocks
            outfitService.UploadImageAsync(
                imageData,
                userId.ToString(),
                Arg.Any<string>(),
                "Outfit",
                "Front")
                .Returns(Result<string>.Success(imageUrl));

            clothingItemRepository.GetByIdAsync(existingItemId).Returns(existingItem);
            clothingItemRepository.GetByIdAsync(nonExistentItemId).ReturnsNull();

            embeddingService.GetEmbeddingAsync("Test outfit").Returns(embedding);

            outfitRepository.AddAsync(Arg.Any<Outfit>())
                .Returns(callInfo =>
                {
                    var outfit = callInfo.Arg<Outfit>();
                    return Result<Guid>.Success(outfit.Id);
                });

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify only the existing item was added to the outfit
            await outfitRepository.Received(1).AddAsync(Arg.Is<Outfit>(o =>
                o.OutfitClothingItems.Count == 1 &&
                o.OutfitClothingItems[0].ClothingItemId == existingItemId
            ));

            // Verify only existing item was updated
            await clothingItemRepository.Received(1).UpdateAsync(Arg.Is<ClothingItem>(c =>
                c.Id == existingItemId
            ));
            await clothingItemRepository.DidNotReceive().UpdateAsync(Arg.Is<ClothingItem>(c =>
                c.Id == nonExistentItemId
            ));
        }

        

        [Fact]
        public async Task Handle_ShouldSetEmptyEmbedding_WhenEmbeddingServiceFails()
        {
            // Arrange
            var userId = Guid.Parse("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d");
            var imageData = new byte[] { 1, 2, 3, 4, 5 };
            var imageUrl = "https://storage.example.com/outfits/image.jpg";

            var command = new CreateOutfitCommand
            {
                UserId = userId,
                Name = "Embedding Fail Outfit",
                Style = "Casual",
                ClothingItemIds = new List<Guid>(),
                Season = "Spring", // Add required properties
                Description = "Test description",
                ImageFront = imageData
            };

            // Mock successful image upload
            outfitService.UploadImageAsync(
                imageData,
                userId.ToString(),
                Arg.Any<string>(),
                "Outfit",
                "Front")
                .Returns(Result<string>.Success(imageUrl));

            // Mock embedding service failure or returns null
            embeddingService.GetEmbeddingAsync("Test description").ReturnsNull();

            outfitRepository.AddAsync(Arg.Any<Outfit>())
                .Returns(callInfo =>
                {
                    var outfit = callInfo.Arg<Outfit>();
                    return Result<Guid>.Success(outfit.Id);
                });

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify outfit was created with null embedding
            await outfitRepository.Received(1).AddAsync(Arg.Is<Outfit>(o =>
                o.Embedding == null
            ));
        }

        [Fact]
        public async Task Handle_ShouldIncrementWearCountForAllItems()
        {
            // Arrange
            var userId = Guid.Parse("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d");
            var clothingItemId1 = Guid.Parse("2a3b4c5d-6e7f-8a9b-0c1d-2e3f4a5b6c7d");
            var clothingItemId2 = Guid.Parse("3a4b5c6d-7e8f-9a0b-1c2d-3e4f5a6b7c8d");

            var imageData = new byte[] { 1, 2, 3, 4, 5 };
            var imageUrl = "https://storage.example.com/outfits/image.jpg";
            var embedding = new float[] { 0.1f, 0.2f, 0.3f };

            var clothingItem1 = new ClothingItem
            {
                Id = clothingItemId1,
                UserId = userId,
                Name = "Hoodie",
                Category = "Tops",
                Color = "Gray",
                Brand = "TestBrand",
                Material = "Cotton",
                FrontImageUrl = "https://example.com/hoodie.jpg",
                Tags = new List<ClothingTag>(),
                NumberOfWears = 3,
                CreatedAt = DateTime.UtcNow,
                LastWornDate = DateTime.UtcNow.AddDays(-10),
                Weight = 0.9m,
                IsSold = false,
                PrintType = null,
                PrintDescription = null,
                Description = "Warm hoodie",
                BackImageUrl = null,
                Embedding = null,
                OutfitClothingItems = new List<OutfitClothingItem>()
            };

            var clothingItem2 = new ClothingItem
            {
                Id = clothingItemId2,
                UserId = userId,
                Name = "Jeans",
                Category = "Bottoms",
                Color = "Blue",
                Brand = "TestBrand",
                Material = "Denim",
                FrontImageUrl = "https://example.com/jeans.jpg",
                Tags = new List<ClothingTag>(),
                NumberOfWears = 5,
                CreatedAt = DateTime.UtcNow,
                LastWornDate = DateTime.UtcNow.AddDays(-5),
                Weight = 1.2m,
                IsSold = false,
                PrintType = null,
                PrintDescription = null,
                Description = "Comfortable jeans",
                BackImageUrl = null,
                Embedding = null,
                OutfitClothingItems = new List<OutfitClothingItem>()
            };

            var command = new CreateOutfitCommand
            {
                UserId = userId,
                Name = "Wear Count Test",
                Style = "Casual",
                ClothingItemIds = new List<Guid> { clothingItemId1, clothingItemId2 },
                Season = "Fall",
                Description = "Test outfit",
                ImageFront = imageData
            };

            // Set up repository and service mocks
            outfitService.UploadImageAsync(
                imageData,
                userId.ToString(),
                Arg.Any<string>(),
                "Outfit",
                "Front")
                .Returns(Result<string>.Success(imageUrl));

            clothingItemRepository.GetByIdAsync(clothingItemId1).Returns(clothingItem1);
            clothingItemRepository.GetByIdAsync(clothingItemId2).Returns(clothingItem2);

            embeddingService.GetEmbeddingAsync("Test outfit").Returns(embedding);

            outfitRepository.AddAsync(Arg.Any<Outfit>())
                .Returns(callInfo =>
                {
                    var outfit = callInfo.Arg<Outfit>();
                    return Result<Guid>.Success(outfit.Id);
                });

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify wear counts were incremented correctly
            await clothingItemRepository.Received(1).UpdateAsync(Arg.Is<ClothingItem>(c =>
                c.Id == clothingItemId1 && c.NumberOfWears == 4
            ));

            await clothingItemRepository.Received(1).UpdateAsync(Arg.Is<ClothingItem>(c =>
                c.Id == clothingItemId2 && c.NumberOfWears == 6
            ));
        }

        [Fact]
        public async Task Handle_ShouldSetCorrectCreatedAtDate()
        {
            // Arrange
            var userId = Guid.Parse("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d");
            var imageData = new byte[] { 1, 2, 3, 4, 5 };
            var imageUrl = "https://storage.example.com/outfits/image.jpg";
            var embedding = new float[] { 0.1f, 0.2f, 0.3f };

            var command = new CreateOutfitCommand
            {
                UserId = userId,
                Name = "Date Test Outfit",
                Style = "Casual",
                ClothingItemIds = new List<Guid>(),
                Season = "Winter",
                Description = "Test outfit",
                ImageFront = imageData
            };

            // Set up repository and service mocks
            outfitService.UploadImageAsync(
                imageData,
                userId.ToString(),
                Arg.Any<string>(),
                "Outfit",
                "Front")
                .Returns(Result<string>.Success(imageUrl));

            embeddingService.GetEmbeddingAsync("Test outfit").Returns(embedding);

            DateTime? capturedCreatedAt = null;
            outfitRepository.AddAsync(Arg.Any<Outfit>())
                .Returns(callInfo =>
                {
                    var outfit = callInfo.Arg<Outfit>();
                    capturedCreatedAt = outfit.CreatedAt;
                    return Result<Guid>.Success(outfit.Id);
                });

            // Act
            var beforeCreation = DateTime.UtcNow;
            var result = await handler.Handle(command, CancellationToken.None);
            var afterCreation = DateTime.UtcNow;

            // Assert
            result.IsSuccess.Should().BeTrue();
            capturedCreatedAt.Should().NotBeNull();
            capturedCreatedAt.Should().BeAfter(beforeCreation.AddSeconds(-1));
            capturedCreatedAt.Should().BeBefore(afterCreation.AddSeconds(1));
        }

        [Fact]
        public async Task Handle_ShouldCreateOutfitWithEmptyClothingItemsList()
        {
            // Arrange
            var userId = Guid.Parse("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d");
            var imageData = new byte[] { 1, 2, 3, 4, 5 };
            var imageUrl = "https://storage.example.com/outfits/image.jpg";
            var embedding = new float[] { 0.1f, 0.2f, 0.3f };

            var command = new CreateOutfitCommand
            {
                UserId = userId,
                Name = "Empty Items Outfit",
                Style = "Formal",
                ClothingItemIds = new List<Guid>(), // Empty list
                Season = "Spring",
                Description = "Test outfit with no items",
                ImageFront = imageData
            };

            // Set up repository and service mocks
            outfitService.UploadImageAsync(
                imageData,
                userId.ToString(),
                Arg.Any<string>(),
                "Outfit",
                "Front")
                .Returns(Result<string>.Success(imageUrl));

            embeddingService.GetEmbeddingAsync("Test outfit with no items").Returns(embedding);

            outfitRepository.AddAsync(Arg.Any<Outfit>())
                .Returns(callInfo =>
                {
                    var outfit = callInfo.Arg<Outfit>();
                    return Result<Guid>.Success(outfit.Id);
                });

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify outfit was created with empty clothing items list
            await outfitRepository.Received(1).AddAsync(Arg.Is<Outfit>(o =>
                o.OutfitClothingItems.Count == 0
            ));

            // Verify no clothing items were updated
            await clothingItemRepository.DidNotReceive().UpdateAsync(Arg.Any<ClothingItem>());
        }

        [Fact]
        public async Task Handle_ShouldNotUpdateSoldItems()
        {
            // Arrange
            var userId = Guid.Parse("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d");
            var availableItemId = Guid.Parse("2a3b4c5d-6e7f-8a9b-0c1d-2e3f4a5b6c7d");
            var soldItemId = Guid.Parse("3a4b5c6d-7e8f-9a0b-1c2d-3e4f5a6b7c8d");

            var imageData = new byte[] { 1, 2, 3, 4, 5 };
            var imageUrl = "https://storage.example.com/outfits/image.jpg";
            var embedding = new float[] { 0.1f, 0.2f, 0.3f };

            var availableItem = new ClothingItem
            {
                Id = availableItemId,
                UserId = userId,
                Name = "Available Shirt",
                Category = "Tops",
                Color = "White",
                Brand = "TestBrand",
                Material = "Cotton",
                FrontImageUrl = "https://example.com/available.jpg",
                Tags = new List<ClothingTag>(),
                NumberOfWears = 2,
                CreatedAt = DateTime.UtcNow,
                LastWornDate = null,
                Weight = 0.5m,
                IsSold = false,
                PrintType = null,
                PrintDescription = null,
                Description = "Available item",
                BackImageUrl = null,
                Embedding = null,
                OutfitClothingItems = new List<OutfitClothingItem>()
            };

            var soldItem = new ClothingItem
            {
                Id = soldItemId,
                UserId = userId,
                Name = "Sold Shirt",
                Category = "Tops",
                Color = "Yellow",
                Brand = "TestBrand",
                Material = "Cotton",
                FrontImageUrl = "https://example.com/sold.jpg",
                Tags = new List<ClothingTag>(),
                NumberOfWears = 10,
                CreatedAt = DateTime.UtcNow,
                LastWornDate = DateTime.UtcNow.AddMonths(-2),
                Weight = 0.5m,
                IsSold = true, // Sold item
                PrintType = null,
                PrintDescription = null,
                Description = "Sold item",
                BackImageUrl = null,
                Embedding = null,
                OutfitClothingItems = new List<OutfitClothingItem>()
            };

            var command = new CreateOutfitCommand
            {
                UserId = userId,
                Name = "Mixed Available and Sold",
                Style = "Casual",
                ClothingItemIds = new List<Guid> { availableItemId, soldItemId },
                Season = "Summer",
                Description = "Test outfit with mixed items",
                ImageFront = imageData
            };

            // Set up repository and service mocks
            outfitService.UploadImageAsync(
                imageData,
                userId.ToString(),
                Arg.Any<string>(),
                "Outfit",
                "Front")
                .Returns(Result<string>.Success(imageUrl));

            clothingItemRepository.GetByIdAsync(availableItemId).Returns(availableItem);
            clothingItemRepository.GetByIdAsync(soldItemId).Returns(soldItem);

            embeddingService.GetEmbeddingAsync("Test outfit with mixed items").Returns(embedding);

            outfitRepository.AddAsync(Arg.Any<Outfit>())
                .Returns(callInfo =>
                {
                    var outfit = callInfo.Arg<Outfit>();
                    return Result<Guid>.Success(outfit.Id);
                });

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify both items were added to the outfit
            await outfitRepository.Received(1).AddAsync(Arg.Is<Outfit>(o =>
                o.OutfitClothingItems.Count == 2 &&
                o.OutfitClothingItems.Any(i => i.ClothingItemId == availableItemId) &&
                o.OutfitClothingItems.Any(i => i.ClothingItemId == soldItemId)
            ));

            // Verify only available item was updated
            await clothingItemRepository.Received(1).UpdateAsync(Arg.Is<ClothingItem>(c =>
                c.Id == availableItemId && c.NumberOfWears == 3
            ));
            await clothingItemRepository.Received(1).UpdateAsync(Arg.Is<ClothingItem>(c =>
                c.Id == soldItemId && c.NumberOfWears == 11
            ));
        }

        [Fact]
        public async Task Handle_ShouldEmbedEmptyString_WhenDescriptionIsEmpty()
        {
            // Arrange
            var userId = Guid.Parse("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d");
            var imageData = new byte[] { 1, 2, 3, 4, 5 };
            var imageUrl = "https://storage.example.com/outfits/image.jpg";
            var embedding = new float[] { 0.0f, 0.0f, 0.0f };

            var command = new CreateOutfitCommand
            {
                UserId = userId,
                Name = "Empty Description Outfit",
                Style = "Casual",
                ClothingItemIds = new List<Guid>(),
                Season = "All Season",
                Description = "", // Empty string
                ImageFront = imageData
            };

            // Set up repository and service mocks
            outfitService.UploadImageAsync(
                imageData,
                userId.ToString(),
                Arg.Any<string>(),
                "Outfit",
                "Front")
                .Returns(Result<string>.Success(imageUrl));

            embeddingService.GetEmbeddingAsync(string.Empty).Returns(embedding);

            outfitRepository.AddAsync(Arg.Any<Outfit>())
                .Returns(callInfo =>
                {
                    var outfit = callInfo.Arg<Outfit>();
                    return Result<Guid>.Success(outfit.Id);
                });

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify embedding service was called with empty string
            await embeddingService.Received(1).GetEmbeddingAsync(string.Empty);

            // Verify outfit was created with the correct embedding
            await outfitRepository.Received(1).AddAsync(Arg.Is<Outfit>(o =>
                o.Embedding == embedding
            ));
        }
    }
}