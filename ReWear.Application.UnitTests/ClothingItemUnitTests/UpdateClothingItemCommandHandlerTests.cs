using Application.Services;
using Application.Use_Cases.CommandHandlers.ClothingItemCommandHandlers;
using Application.Use_Cases.Commands.ClothingItemCommand;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Models;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace ReWear.Application.UnitTests.ClothingItemUnitTests
{
    public class UpdateClothingItemCommandHandlerTests
    {
        private readonly IClothingItemRepository clothingItemRepository;
        private readonly IMapper mapper;
        private readonly IClothingItemService clothingItemService;
        private readonly UpdateClothingItemCommandHandler handler;

        public UpdateClothingItemCommandHandlerTests()
        {
            this.clothingItemRepository = Substitute.For<IClothingItemRepository>();
            this.mapper = Substitute.For<IMapper>();
            this.clothingItemService = Substitute.For<IClothingItemService>();
            this.handler = new UpdateClothingItemCommandHandler(clothingItemRepository, mapper, clothingItemService);
        }

        [Fact]
        public async Task Given_ValidUpdateClothingItemCommand_When_HandlerIsCalled_Then_ClothingItemIsUpdatedSuccessfully()
        {
            // Arrange
            var clothingItemId = Guid.Parse("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var userId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b");
            var command = new UpdateClothingItemCommand
            {
                Id = clothingItemId,
                UserId = userId,
                Name = "Updated Blue Jeans",
                Category = "Bottoms",
                Tags = new List<string> { "Casual", "Denim", "Updated" },
                Color = "Dark Blue",
                Brand = "Levi's",
                Material = "Denim",
                PrintType = "Pattern",
                PrintDescription = "Classic design",
                Description = "Updated description"
            };

            var existingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = userId,
                Name = "Blue Jeans",
                Category = "Bottoms",
                Tags = new List<ClothingTag> { new ClothingTag { Tag = "Casual" } },
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                PrintType = null,
                PrintDescription = null,
                Description = "Original description",
                FrontImageUrl = "https://storage.url/original-front-image.jpg",
                BackImageUrl = "https://storage.url/original-back-image.jpg",
                Embedding = new float[] { 0.1f, 0.2f, 0.3f },
                CreatedAt = DateTime.Parse("2023-05-10"),
                NumberOfWears = 5,
                LastWornDate = DateTime.Parse("2023-06-15"),
                Weight = 0.5m,
                IsSold = false,
                OutfitClothingItems = new List<OutfitClothingItem>()
            };

            clothingItemRepository.GetByIdAsync(clothingItemId).Returns(existingItem);
            clothingItemRepository.UpdateAsync(Arg.Any<ClothingItem>())
                .Returns(Result<string>.Success("Clothing item updated successfully"));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be("Clothing item updated successfully");
            await clothingItemRepository.Received(1).UpdateAsync(Arg.Is<ClothingItem>(item =>
                item.Id == clothingItemId &&
                item.UserId == userId &&
                item.Name == command.Name &&
                item.Category == command.Category &&
                item.Color == command.Color &&
                item.Brand == command.Brand &&
                item.Material == command.Material &&
                item.PrintType == command.PrintType &&
                item.PrintDescription == command.PrintDescription &&
                item.Description == command.Description &&
                item.FrontImageUrl == existingItem.FrontImageUrl &&
                item.BackImageUrl == existingItem.BackImageUrl &&
                item.Embedding == existingItem.Embedding &&
                item.CreatedAt == existingItem.CreatedAt &&
                item.NumberOfWears == existingItem.NumberOfWears &&
                item.LastWornDate == existingItem.LastWornDate &&
                item.Weight == existingItem.Weight &&
                item.IsSold == existingItem.IsSold &&
                item.Tags.Count == 3
            ));
        }

        [Fact]
        public async Task Given_UpdateWithFrontImage_When_HandlerIsCalled_Then_FrontImageIsUpdated()
        {
            // Arrange
            var clothingItemId = Guid.Parse("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var userId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b");
            var frontImageData = new byte[] { 1, 2, 3, 4, 5 };
            var command = new UpdateClothingItemCommand
            {
                Id = clothingItemId,
                UserId = userId,
                Name = "Updated Blue Jeans",
                Category = "Bottoms",
                Tags = new List<string> { "Casual", "Denim" },
                Color = "Dark Blue",
                Brand = "Levi's",
                Material = "Denim",
                PrintType = "Pattern",
                PrintDescription = "Classic design",
                Description = "Updated description",
                ImageFront = frontImageData
            };

            var existingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = userId,
                Name = "Blue Jeans",
                Category = "Bottoms",
                Tags = new List<ClothingTag> { new ClothingTag { Tag = "Casual" } },
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                PrintType = null,
                PrintDescription = null,
                Description = "Original description",
                FrontImageUrl = "https://storage.url/original-front-image.jpg",
                BackImageUrl = "https://storage.url/original-back-image.jpg",
                Embedding = new float[] { 0.1f, 0.2f, 0.3f },
                CreatedAt = DateTime.Parse("2023-05-10"),
                NumberOfWears = 5,
                LastWornDate = DateTime.Parse("2023-06-15"),
                Weight = 0.5m,
                IsSold = false,
                OutfitClothingItems = new List<OutfitClothingItem>()
            };

            var newFrontImageUrl = "https://storage.url/new-front-image.jpg";

            clothingItemRepository.GetByIdAsync(clothingItemId).Returns(existingItem);
            clothingItemService.UploadImageAsync(frontImageData, userId.ToString(), clothingItemId.ToString(), "ClothingItem", "Front")
                .Returns(Result<string>.Success(newFrontImageUrl));
            clothingItemRepository.UpdateAsync(Arg.Any<ClothingItem>())
                .Returns(Result<string>.Success("Clothing item updated successfully"));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            await clothingItemRepository.Received(1).UpdateAsync(Arg.Is<ClothingItem>(item =>
                item.FrontImageUrl == newFrontImageUrl &&
                item.BackImageUrl == existingItem.BackImageUrl
            ));
        }

        [Fact]
        public async Task Given_UpdateWithBackImage_When_HandlerIsCalled_Then_BackImageIsUpdated()
        {
            // Arrange
            var clothingItemId = Guid.Parse("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var userId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b");
            var backImageData = new byte[] { 6, 7, 8, 9, 10 };
            var command = new UpdateClothingItemCommand
            {
                Id = clothingItemId,
                UserId = userId,
                Name = "Updated Blue Jeans",
                Category = "Bottoms",
                Tags = new List<string> { "Casual", "Denim" },
                Color = "Dark Blue",
                Brand = "Levi's",
                Material = "Denim",
                PrintType = "Pattern",
                PrintDescription = "Classic design",
                Description = "Updated description",
                ImageBack = backImageData
            };

            var existingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = userId,
                Name = "Blue Jeans",
                Category = "Bottoms",
                Tags = new List<ClothingTag> { new ClothingTag { Tag = "Casual" } },
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                PrintType = null,
                PrintDescription = null,
                Description = "Original description",
                FrontImageUrl = "https://storage.url/original-front-image.jpg",
                BackImageUrl = "https://storage.url/original-back-image.jpg",
                Embedding = new float[] { 0.1f, 0.2f, 0.3f },
                CreatedAt = DateTime.Parse("2023-05-10"),
                NumberOfWears = 5,
                LastWornDate = DateTime.Parse("2023-06-15"),
                Weight = 0.5m,
                IsSold = false,
                OutfitClothingItems = new List<OutfitClothingItem>()
            };

            var newBackImageUrl = "https://storage.url/new-back-image.jpg";

            clothingItemRepository.GetByIdAsync(clothingItemId).Returns(existingItem);
            clothingItemService.UploadImageAsync(backImageData, userId.ToString(), clothingItemId.ToString(), "ClothingItem", "Back")
                .Returns(Result<string>.Success(newBackImageUrl));
            clothingItemRepository.UpdateAsync(Arg.Any<ClothingItem>())
                .Returns(Result<string>.Success("Clothing item updated successfully"));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            await clothingItemRepository.Received(1).UpdateAsync(Arg.Is<ClothingItem>(item =>
                item.FrontImageUrl == existingItem.FrontImageUrl &&
                item.BackImageUrl == newBackImageUrl
            ));
        }

        [Fact]
        public async Task Given_UpdateWithBothImages_When_HandlerIsCalled_Then_BothImagesAreUpdated()
        {
            // Arrange
            var clothingItemId = Guid.Parse("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var userId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b");
            var frontImageData = new byte[] { 1, 2, 3, 4, 5 };
            var backImageData = new byte[] { 6, 7, 8, 9, 10 };
            var command = new UpdateClothingItemCommand
            {
                Id = clothingItemId,
                UserId = userId,
                Name = "Updated Blue Jeans",
                Category = "Bottoms",
                Tags = new List<string> { "Casual", "Denim" },
                Color = "Dark Blue",
                Brand = "Levi's",
                Material = "Denim",
                PrintType = "Pattern",
                PrintDescription = "Classic design",
                Description = "Updated description",
                ImageFront = frontImageData,
                ImageBack = backImageData
            };

            var existingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = userId,
                Name = "Blue Jeans",
                Category = "Bottoms",
                Tags = new List<ClothingTag> { new ClothingTag { Tag = "Casual" } },
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                PrintType = null,
                PrintDescription = null,
                Description = "Original description",
                FrontImageUrl = "https://storage.url/original-front-image.jpg",
                BackImageUrl = "https://storage.url/original-back-image.jpg",
                Embedding = new float[] { 0.1f, 0.2f, 0.3f },
                CreatedAt = DateTime.Parse("2023-05-10"),
                NumberOfWears = 5,
                LastWornDate = DateTime.Parse("2023-06-15"),
                Weight = 0.5m,
                IsSold = false,
                OutfitClothingItems = new List<OutfitClothingItem>()
            };

            var newFrontImageUrl = "https://storage.url/new-front-image.jpg";
            var newBackImageUrl = "https://storage.url/new-back-image.jpg";

            clothingItemRepository.GetByIdAsync(clothingItemId).Returns(existingItem);
            clothingItemService.UploadImageAsync(frontImageData, userId.ToString(), clothingItemId.ToString(), "ClothingItem", "Front")
                .Returns(Result<string>.Success(newFrontImageUrl));
            clothingItemService.UploadImageAsync(backImageData, userId.ToString(), clothingItemId.ToString(), "ClothingItem", "Back")
                .Returns(Result<string>.Success(newBackImageUrl));
            clothingItemRepository.UpdateAsync(Arg.Any<ClothingItem>())
                .Returns(Result<string>.Success("Clothing item updated successfully"));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            await clothingItemRepository.Received(1).UpdateAsync(Arg.Is<ClothingItem>(item =>
                item.FrontImageUrl == newFrontImageUrl &&
                item.BackImageUrl == newBackImageUrl
            ));
        }

        [Fact]
        public async Task Given_NonExistentClothingItemId_When_HandlerIsCalled_Then_FailureIsReturned()
        {
            // Arrange
            var clothingItemId = Guid.Parse("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var command = new UpdateClothingItemCommand
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Updated Blue Jeans",
                Category = "Bottoms",
                Tags = new List<string> { "Casual", "Denim" },
                Color = "Dark Blue",
                Brand = "Levi's",
                Material = "Denim"
            };

            clothingItemRepository.GetByIdAsync(clothingItemId).Returns((ClothingItem)null);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Clothing item not found");
            await clothingItemRepository.DidNotReceive().UpdateAsync(Arg.Any<ClothingItem>());
        }

        [Fact]
        public async Task Given_EmptyClothingItemId_When_HandlerIsCalled_Then_ValidationFailure()
        {
            // Arrange
            var command = new UpdateClothingItemCommand
            {
                Id = Guid.Empty,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Updated Blue Jeans",
                Category = "Bottoms",
                Color = "Dark Blue",
                Brand = "Levi's",
                Material = "Denim"
            };

            clothingItemRepository.GetByIdAsync(Guid.Empty)
                .Returns((ClothingItem)null);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Clothing item not found");
        }

        [Fact]
        public async Task Given_EmptyUserId_When_HandlerIsCalled_Then_ValidationFailure()
        {
            // Arrange
            var clothingItemId = Guid.Parse("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var command = new UpdateClothingItemCommand
            {
                Id = clothingItemId,
                UserId = Guid.Empty,
                Name = "Updated Blue Jeans",
                Category = "Bottoms",
                Color = "Dark Blue",
                Brand = "Levi's",
                Material = "Denim"
            };

            var existingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Blue Jeans",
                Category = "Bottoms",
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                FrontImageUrl = "https://storage.url/original-front-image.jpg"
            };

            clothingItemRepository.GetByIdAsync(clothingItemId).Returns(existingItem);
            clothingItemRepository.UpdateAsync(Arg.Any<ClothingItem>())
                .Returns(Result<string>.Failure("User ID is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User ID is required.");
        }

        [Fact]
        public async Task Given_EmptyName_When_HandlerIsCalled_Then_ValidationFailure()
        {
            // Arrange
            var clothingItemId = Guid.Parse("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var command = new UpdateClothingItemCommand
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "",
                Category = "Bottoms",
                Color = "Dark Blue",
                Brand = "Levi's",
                Material = "Denim"
            };

            var existingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Blue Jeans",
                Category = "Bottoms",
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                FrontImageUrl = "https://storage.url/original-front-image.jpg"
            };

            clothingItemRepository.GetByIdAsync(clothingItemId).Returns(existingItem);
            clothingItemRepository.UpdateAsync(Arg.Any<ClothingItem>())
                .Returns(Result<string>.Failure("Name is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Name is required.");
        }

        [Fact]
        public async Task Given_NameTooLong_When_HandlerIsCalled_Then_ValidationFailure()
        {
            // Arrange
            var clothingItemId = Guid.Parse("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var command = new UpdateClothingItemCommand
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = new string('A', 51), // Name exceeds 50 characters
                Category = "Bottoms",
                Color = "Dark Blue",
                Brand = "Levi's",
                Material = "Denim"
            };

            var existingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Blue Jeans",
                Category = "Bottoms",
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                FrontImageUrl = "https://storage.url/original-front-image.jpg"
            };

            clothingItemRepository.GetByIdAsync(clothingItemId).Returns(existingItem);
            clothingItemRepository.UpdateAsync(Arg.Any<ClothingItem>())
                .Returns(Result<string>.Failure("Name cannot exceed 50 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Name cannot exceed 50 characters.");
        }

        [Fact]
        public async Task Given_EmptyCategory_When_HandlerIsCalled_Then_ValidationFailure()
        {
            // Arrange
            var clothingItemId = Guid.Parse("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var command = new UpdateClothingItemCommand
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Updated Blue Jeans",
                Category = "",
                Color = "Dark Blue",
                Brand = "Levi's",
                Material = "Denim"
            };

            var existingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Blue Jeans",
                Category = "Bottoms",
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                FrontImageUrl = "https://storage.url/original-front-image.jpg"
            };

            clothingItemRepository.GetByIdAsync(clothingItemId).Returns(existingItem);
            clothingItemRepository.UpdateAsync(Arg.Any<ClothingItem>())
                .Returns(Result<string>.Failure("Category is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Category is required.");
        }

        [Fact]
        public async Task Given_CategoryTooLong_When_HandlerIsCalled_Then_ValidationFailure()
        {
            // Arrange
            var clothingItemId = Guid.Parse("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var command = new UpdateClothingItemCommand
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Updated Blue Jeans",
                Category = new string('A', 51), // Category exceeds 50 characters
                Color = "Dark Blue",
                Brand = "Levi's",
                Material = "Denim"
            };

            var existingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Blue Jeans",
                Category = "Bottoms",
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                FrontImageUrl = "https://storage.url/original-front-image.jpg"
            };

            clothingItemRepository.GetByIdAsync(clothingItemId).Returns(existingItem);
            clothingItemRepository.UpdateAsync(Arg.Any<ClothingItem>())
                .Returns(Result<string>.Failure("Category cannot exceed 50 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Category cannot exceed 50 characters.");
        }

        [Fact]
        public async Task Given_TagTooLong_When_HandlerIsCalled_Then_ValidationFailure()
        {
            // Arrange
            var clothingItemId = Guid.Parse("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var command = new UpdateClothingItemCommand
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Updated Blue Jeans",
                Category = "Bottoms",
                Tags = new List<string> { "Casual", new string('A', 31) }, // Tag exceeds 30 characters
                Color = "Dark Blue",
                Brand = "Levi's",
                Material = "Denim"
            };

            var existingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Blue Jeans",
                Category = "Bottoms",
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                FrontImageUrl = "https://storage.url/original-front-image.jpg"
            };

            clothingItemRepository.GetByIdAsync(clothingItemId).Returns(existingItem);
            clothingItemRepository.UpdateAsync(Arg.Any<ClothingItem>())
                .Returns(Result<string>.Failure("Each tag cannot exceed 30 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Each tag cannot exceed 30 characters.");
        }

        [Fact]
        public async Task Given_EmptyColor_When_HandlerIsCalled_Then_ValidationFailure()
        {
            // Arrange
            var clothingItemId = Guid.Parse("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var command = new UpdateClothingItemCommand
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Updated Blue Jeans",
                Category = "Bottoms",
                Color = "",
                Brand = "Levi's",
                Material = "Denim"
            };

            var existingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Blue Jeans",
                Category = "Bottoms",
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                FrontImageUrl = "https://storage.url/original-front-image.jpg"
            };

            clothingItemRepository.GetByIdAsync(clothingItemId).Returns(existingItem);
            clothingItemRepository.UpdateAsync(Arg.Any<ClothingItem>())
                .Returns(Result<string>.Failure("Color is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Color is required.");
        }

        [Fact]
        public async Task Given_ColorTooLong_When_HandlerIsCalled_Then_ValidationFailure()
        {
            // Arrange
            var clothingItemId = Guid.Parse("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var command = new UpdateClothingItemCommand
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Updated Blue Jeans",
                Category = "Bottoms",
                Color = new string('A', 21), // Color exceeds 20 characters
                Brand = "Levi's",
                Material = "Denim"
            };

            var existingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Blue Jeans",
                Category = "Bottoms",
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                FrontImageUrl = "https://storage.url/original-front-image.jpg"
            };

            clothingItemRepository.GetByIdAsync(clothingItemId).Returns(existingItem);
            clothingItemRepository.UpdateAsync(Arg.Any<ClothingItem>())
                .Returns(Result<string>.Failure("Color cannot exceed 20 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Color cannot exceed 20 characters.");
        }

        [Fact]
        public async Task Given_EmptyBrand_When_HandlerIsCalled_Then_ValidationFailure()
        {
            // Arrange
            var clothingItemId = Guid.Parse("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var command = new UpdateClothingItemCommand
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Updated Blue Jeans",
                Category = "Bottoms",
                Color = "Dark Blue",
                Brand = "",
                Material = "Denim"
            };

            var existingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Blue Jeans",
                Category = "Bottoms",
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                FrontImageUrl = "https://storage.url/original-front-image.jpg"
            };

            clothingItemRepository.GetByIdAsync(clothingItemId).Returns(existingItem);
            clothingItemRepository.UpdateAsync(Arg.Any<ClothingItem>())
                .Returns(Result<string>.Failure("Brand is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Brand is required.");
        }

        [Fact]
        public async Task Given_BrandTooLong_When_HandlerIsCalled_Then_ValidationFailure()
        {
            // Arrange
            var clothingItemId = Guid.Parse("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var command = new UpdateClothingItemCommand
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Updated Blue Jeans",
                Category = "Bottoms",
                Color = "Dark Blue",
                Brand = new string('A', 31), // Brand exceeds 30 characters
                Material = "Denim"
            };

            var existingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Blue Jeans",
                Category = "Bottoms",
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                FrontImageUrl = "https://storage.url/original-front-image.jpg"
            };

            clothingItemRepository.GetByIdAsync(clothingItemId).Returns(existingItem);
            clothingItemRepository.UpdateAsync(Arg.Any<ClothingItem>())
                .Returns(Result<string>.Failure("Brand cannot exceed 30 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Brand cannot exceed 30 characters.");
        }

        [Fact]
        public async Task Given_EmptyMaterial_When_HandlerIsCalled_Then_ValidationFailure()
        {
            // Arrange
            var clothingItemId = Guid.Parse("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var command = new UpdateClothingItemCommand
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Updated Blue Jeans",
                Category = "Bottoms",
                Color = "Dark Blue",
                Brand = "Levi's",
                Material = ""
            };

            var existingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Blue Jeans",
                Category = "Bottoms",
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                FrontImageUrl = "https://storage.url/original-front-image.jpg"
            };

            clothingItemRepository.GetByIdAsync(clothingItemId).Returns(existingItem);
            clothingItemRepository.UpdateAsync(Arg.Any<ClothingItem>())
                .Returns(Result<string>.Failure("Material is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Material is required.");
        }

        [Fact]
        public async Task Given_MaterialTooLong_When_HandlerIsCalled_Then_ValidationFailure()
        {
            // Arrange
            var clothingItemId = Guid.Parse("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var command = new UpdateClothingItemCommand
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Updated Blue Jeans",
                Category = "Bottoms",
                Color = "Dark Blue",
                Brand = "Levi's",
                Material = new string('A', 31) // Material exceeds 30 characters
            };

            var existingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Blue Jeans",
                Category = "Bottoms",
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                FrontImageUrl = "https://storage.url/original-front-image.jpg"
            };

            clothingItemRepository.GetByIdAsync(clothingItemId).Returns(existingItem);
            clothingItemRepository.UpdateAsync(Arg.Any<ClothingItem>())
                .Returns(Result<string>.Failure("Material cannot exceed 30 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Material cannot exceed 30 characters.");
        }

        [Fact]
        public async Task Given_PrintTypeTooLong_When_HandlerIsCalled_Then_ValidationFailure()
        {
            // Arrange
            var clothingItemId = Guid.Parse("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var command = new UpdateClothingItemCommand
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Updated T-Shirt",
                Category = "Tops",
                Color = "White",
                Brand = "Nike",
                Material = "Cotton",
                PrintType = new string('A', 31) // PrintType exceeds 30 characters
            };

            var existingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "T-Shirt",
                Category = "Tops",
                Color = "White",
                Brand = "Nike",
                Material = "Cotton",
                PrintType = "Logo",
                FrontImageUrl = "https://storage.url/original-front-image.jpg"
            };

            clothingItemRepository.GetByIdAsync(clothingItemId).Returns(existingItem);
            clothingItemRepository.UpdateAsync(Arg.Any<ClothingItem>())
                .Returns(Result<string>.Failure("Print type cannot exceed 30 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Print type cannot exceed 30 characters.");
        }

        [Fact]
        public async Task Given_PrintDescriptionTooLong_When_HandlerIsCalled_Then_ValidationFailure()
        {
            // Arrange
            var clothingItemId = Guid.Parse("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var command = new UpdateClothingItemCommand
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Updated T-Shirt",
                Category = "Tops",
                Color = "White",
                Brand = "Nike",
                Material = "Cotton",
                PrintType = "Logo",
                PrintDescription = new string('A', 101) // PrintDescription exceeds 100 characters
            };

            var existingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "T-Shirt",
                Category = "Tops",
                Color = "White",
                Brand = "Nike",
                Material = "Cotton",
                PrintType = "Logo",
                PrintDescription = "Simple brand logo",
                FrontImageUrl = "https://storage.url/original-front-image.jpg"
            };

            clothingItemRepository.GetByIdAsync(clothingItemId).Returns(existingItem);
            clothingItemRepository.UpdateAsync(Arg.Any<ClothingItem>())
                .Returns(Result<string>.Failure("Print description cannot exceed 100 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Print description cannot exceed 100 characters.");
        }

        [Fact]
        public async Task Given_DescriptionTooLong_When_HandlerIsCalled_Then_ValidationFailure()
        {
            // Arrange
            var clothingItemId = Guid.Parse("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var command = new UpdateClothingItemCommand
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "Updated T-Shirt",
                Category = "Tops",
                Color = "White",
                Brand = "Nike",
                Material = "Cotton",
                Description = new string('A', 2001) // Description exceeds 2000 characters
            };

            var existingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = Guid.Parse("a5e0d4c3-b2f1-4e6d-8a7b-9c0e1d3f2a5b"),
                Name = "T-Shirt",
                Category = "Tops",
                Color = "White",
                Brand = "Nike",
                Material = "Cotton",
                Description = "Simple white t-shirt",
                FrontImageUrl = "https://storage.url/original-front-image.jpg"
            };

            clothingItemRepository.GetByIdAsync(clothingItemId).Returns(existingItem);
            clothingItemRepository.UpdateAsync(Arg.Any<ClothingItem>())
                .Returns(Result<string>.Failure("Description cannot exceed 2000 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Description cannot exceed 2000 characters.");
        }
    }
}