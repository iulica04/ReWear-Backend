using Application.Services;
using Application.Use_Cases.CommandHandlers.ClothingItemCommandHandlers;
using Application.Use_Cases.Commands.ClothingItemCommand;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace ReWear.Application.UnitTests.ClothingItemUnitTests
{
    public class CreateClothingItemCommandHandlerTests
    {
        private readonly IClothingItemRepository clothingItemRepository;
        private readonly IClothingItemService clothingItemService;
        private readonly IEmbeddingService embeddingService;
        private readonly CreateClothingItemCommandHandler handler;

        public CreateClothingItemCommandHandlerTests()
        {
            this.clothingItemRepository = Substitute.For<IClothingItemRepository>();
            this.clothingItemService = Substitute.For<IClothingItemService>();
            this.embeddingService = Substitute.For<IEmbeddingService>();
            this.handler = new CreateClothingItemCommandHandler(clothingItemRepository, clothingItemService, embeddingService);
        }

        [Fact]
        public async Task Given_ValidCreateClothingItemCommand_When_HandlerIsCalled_Then_ClothingItemShouldBeCreated()
        {
            // Arrange
            var clothingItemId = Guid.NewGuid();
            var command = new CreateClothingItemCommand
            {
                UserId = Guid.NewGuid(),
                Name = "Blue Jeans",
                Category = "Bottoms",
                Tags = new List<string> { "Casual", "Denim" },
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                Description = "Classic blue jeans",
                ImageFront = new byte[] { 1, 2, 3, 4, 5 } // Sample image data
            };

            var frontImageUrl = "https://storage.url/front-image.jpg";
            clothingItemService.UploadImageAsync(
                command.ImageFront,
                command.UserId.ToString(),
                Arg.Any<string>(),
                "ClothingItem",
                "Front")
                .Returns(Result<string>.Success(frontImageUrl));

            embeddingService.GetEmbeddingAsync(command.Description)
                .Returns(new float[] { 0.1f, 0.2f, 0.3f });

            clothingItemRepository.AddAsync(Arg.Any<ClothingItem>())
                .Returns(Result<Guid>.Success(clothingItemId));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeEmpty();
            await clothingItemRepository.Received(1).AddAsync(Arg.Is<ClothingItem>(item =>
                item.Name == command.Name &&
                item.Category == command.Category &&
                item.Color == command.Color &&
                item.Brand == command.Brand &&
                item.Material == command.Material &&
                item.Description == command.Description &&
                item.FrontImageUrl == frontImageUrl &&
                item.NumberOfWears == 0
            ));
        }

        [Fact]
        public async Task Given_CommandWithBackImage_When_HandlerIsCalled_Then_BackImageShouldBeUploaded()
        {
            // Arrange
            var clothingItemId = Guid.NewGuid();
            var command = new CreateClothingItemCommand
            {
                UserId = Guid.NewGuid(),
                Name = "T-Shirt",
                Category = "Tops",
                Tags = new List<string> { "Casual", "Summer" },
                Color = "White",
                Brand = "Nike",
                Material = "Cotton",
                Description = "Basic white t-shirt",
                ImageFront = new byte[] { 1, 2, 3, 4, 5 }, // Sample front image data
                ImageBack = new byte[] { 6, 7, 8, 9, 10 } // Sample back image data
            };

            var frontImageUrl = "https://storage.url/front-image.jpg";
            var backImageUrl = "https://storage.url/back-image.jpg";

            clothingItemService.UploadImageAsync(
                command.ImageFront,
                command.UserId.ToString(),
                Arg.Any<string>(),
                "ClothingItem",
                "Front")
                .Returns(Result<string>.Success(frontImageUrl));

            clothingItemService.UploadImageAsync(
                command.ImageBack,
                command.UserId.ToString(),
                Arg.Any<string>(),
                "ClothingItem",
                "Back")
                .Returns(Result<string>.Success(backImageUrl));

            embeddingService.GetEmbeddingAsync(command.Description)
                .Returns(new float[] { 0.1f, 0.2f, 0.3f });

            clothingItemRepository.AddAsync(Arg.Any<ClothingItem>())
                .Returns(Result<Guid>.Success(clothingItemId));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            await clothingItemRepository.Received(1).AddAsync(Arg.Is<ClothingItem>(item =>
                item.FrontImageUrl == frontImageUrl &&
                item.BackImageUrl == backImageUrl
            ));
        }

        [Fact]
        public async Task Given_CommandWithTags_When_HandlerIsCalled_Then_TagsShouldBeCreated()
        {
            // Arrange
            var clothingItemId = Guid.NewGuid();
            var command = new CreateClothingItemCommand
            {
                UserId = Guid.NewGuid(),
                Name = "Summer Dress",
                Category = "Dresses",
                Tags = new List<string> { "Summer", "Casual", "Floral" },
                Color = "Yellow",
                Brand = "Zara",
                Material = "Cotton",
                Description = "Floral summer dress",
                ImageFront = new byte[] { 1, 2, 3, 4, 5 }
            };

            clothingItemService.UploadImageAsync(
                command.ImageFront,
                command.UserId.ToString(),
                Arg.Any<string>(),
                "ClothingItem",
                "Front")
                .Returns(Result<string>.Success("https://storage.url/front-image.jpg"));

            embeddingService.GetEmbeddingAsync(command.Description)
                .Returns(new float[] { 0.1f, 0.2f, 0.3f });

            clothingItemRepository.AddAsync(Arg.Any<ClothingItem>())
                .Returns(Result<Guid>.Success(clothingItemId));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            await clothingItemRepository.Received(1).AddAsync(Arg.Is<ClothingItem>(item =>
                item.Tags.Count == 3 &&
                item.Tags.Any(t => t.Tag == "Summer") &&
                item.Tags.Any(t => t.Tag == "Casual") &&
                item.Tags.Any(t => t.Tag == "Floral")
            ));
        }

        [Fact]
        public async Task Given_EmptyUserId_When_HandlerIsCalled_Then_ShouldFailValidation()
        {
            var command = new CreateClothingItemCommand
            {
                UserId = Guid.Empty,
                Name = "Blue Jeans",
                Category = "Bottoms",
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                ImageFront = new byte[] { 1, 2, 3, 4, 5 }
            };

            var frontImageUrl = "https://storage.url/front-image.jpg";
            clothingItemService.UploadImageAsync(
                command.ImageFront,
                command.UserId.ToString(),
                Arg.Any<string>(),
                "ClothingItem",
                "Front")
                .Returns(Result<string>.Success(frontImageUrl));

            embeddingService.GetEmbeddingAsync(Arg.Any<string>())
                .Returns(new float[] { 0.1f, 0.2f, 0.3f });

            clothingItemRepository.AddAsync(Arg.Any<ClothingItem>())
                .Returns(Result<Guid>.Failure("User ID is required."));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User ID is required.");
        }

        [Fact]
        public async Task Given_EmptyName_When_HandlerIsCalled_Then_ShouldFailValidation()
        {
            var command = new CreateClothingItemCommand
            {
                UserId = Guid.NewGuid(),
                Name = "",
                Category = "Bottoms",
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                ImageFront = new byte[] { 1, 2, 3, 4, 5 }
            };

            var frontImageUrl = "https://storage.url/front-image.jpg";
            clothingItemService.UploadImageAsync(
                command.ImageFront,
                command.UserId.ToString(),
                Arg.Any<string>(),
                "ClothingItem",
                "Front")
                .Returns(Result<string>.Success(frontImageUrl));

            embeddingService.GetEmbeddingAsync(Arg.Any<string>())
                .Returns(new float[] { 0.1f, 0.2f, 0.3f });

            clothingItemRepository.AddAsync(Arg.Any<ClothingItem>())
                .Returns(Result<Guid>.Failure("Name is required."));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Name is required.");
        }

        [Fact]
        public async Task Given_TooLongName_When_HandlerIsCalled_Then_ShouldFailValidation()
        {
            var command = new CreateClothingItemCommand
            {
                UserId = Guid.NewGuid(),
                Name = new string('A', 51),
                Category = "Bottoms",
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                ImageFront = new byte[] { 1, 2, 3, 4, 5 }
            };

            var frontImageUrl = "https://storage.url/front-image.jpg";
            clothingItemService.UploadImageAsync(
                command.ImageFront,
                command.UserId.ToString(),
                Arg.Any<string>(),
                "ClothingItem",
                "Front")
                .Returns(Result<string>.Success(frontImageUrl));

            embeddingService.GetEmbeddingAsync(Arg.Any<string>())
                .Returns(new float[] { 0.1f, 0.2f, 0.3f });

            clothingItemRepository.AddAsync(Arg.Any<ClothingItem>())
                .Returns(Result<Guid>.Failure("Name cannot exceed 50 characters."));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Name cannot exceed 50 characters.");
        }

        [Fact]
        public async Task Given_EmptyCategory_When_HandlerIsCalled_Then_ShouldFailValidation()
        {
            var command = new CreateClothingItemCommand
            {
                UserId = Guid.NewGuid(),
                Name = "Blue Jeans",
                Category = "",
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                ImageFront = new byte[] { 1, 2, 3, 4, 5 }
            };

            var frontImageUrl = "https://storage.url/front-image.jpg";
            clothingItemService.UploadImageAsync(
                command.ImageFront,
                command.UserId.ToString(),
                Arg.Any<string>(),
                "ClothingItem",
                "Front")
                .Returns(Result<string>.Success(frontImageUrl));

            embeddingService.GetEmbeddingAsync(Arg.Any<string>())
                .Returns(new float[] { 0.1f, 0.2f, 0.3f });

            clothingItemRepository.AddAsync(Arg.Any<ClothingItem>())
                .Returns(Result<Guid>.Failure("Category is required."));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Category is required.");
        }

        [Fact]
        public async Task Given_TooLongCategory_When_HandlerIsCalled_Then_ShouldFailValidation()
        {
            var command = new CreateClothingItemCommand
            {
                UserId = Guid.NewGuid(),
                Name = "Blue Jeans",
                Category = new string('A', 51),
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                ImageFront = new byte[] { 1, 2, 3, 4, 5 }
            };

            var frontImageUrl = "https://storage.url/front-image.jpg";
            clothingItemService.UploadImageAsync(
                command.ImageFront,
                command.UserId.ToString(),
                Arg.Any<string>(),
                "ClothingItem",
                "Front")
                .Returns(Result<string>.Success(frontImageUrl));

            embeddingService.GetEmbeddingAsync(Arg.Any<string>())
                .Returns(new float[] { 0.1f, 0.2f, 0.3f });

            clothingItemRepository.AddAsync(Arg.Any<ClothingItem>())
                .Returns(Result<Guid>.Failure("Category cannot exceed 50 characters."));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Category cannot exceed 50 characters.");
        }

        [Fact]
        public async Task Given_TagsExceedingMaxLength_When_HandlerIsCalled_Then_ShouldFailValidation()
        {
            var command = new CreateClothingItemCommand
            {
                UserId = Guid.NewGuid(),
                Name = "Blue Jeans",
                Category = "Bottoms",
                Tags = new List<string> { new string('A', 31) },
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                ImageFront = new byte[] { 1, 2, 3, 4, 5 }
            };

            var frontImageUrl = "https://storage.url/front-image.jpg";
            clothingItemService.UploadImageAsync(
                command.ImageFront,
                command.UserId.ToString(),
                Arg.Any<string>(),
                "ClothingItem",
                "Front")
                .Returns(Result<string>.Success(frontImageUrl));

            embeddingService.GetEmbeddingAsync(Arg.Any<string>())
                .Returns(new float[] { 0.1f, 0.2f, 0.3f });

            clothingItemRepository.AddAsync(Arg.Any<ClothingItem>())
                .Returns(Result<Guid>.Failure("Each tag cannot exceed 30 characters."));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Each tag cannot exceed 30 characters.");
        }

        [Fact]
        public async Task Given_EmptyColor_When_HandlerIsCalled_Then_ShouldFailValidation()
        {
            var command = new CreateClothingItemCommand
            {
                UserId = Guid.NewGuid(),
                Name = "Blue Jeans",
                Category = "Bottoms",
                Color = "",
                Brand = "Levi's",
                Material = "Cotton",
                ImageFront = new byte[] { 1, 2, 3, 4, 5 }
            };

            var frontImageUrl = "https://storage.url/front-image.jpg";
            clothingItemService.UploadImageAsync(
                command.ImageFront,
                command.UserId.ToString(),
                Arg.Any<string>(),
                "ClothingItem",
                "Front")
                .Returns(Result<string>.Success(frontImageUrl));

            embeddingService.GetEmbeddingAsync(Arg.Any<string>())
                .Returns(new float[] { 0.1f, 0.2f, 0.3f });

            clothingItemRepository.AddAsync(Arg.Any<ClothingItem>())
                .Returns(Result<Guid>.Failure("Color is required."));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Color is required.");
        }

        [Fact]
        public async Task Given_TooLongColor_When_HandlerIsCalled_Then_ShouldFailValidation()
        {
            var command = new CreateClothingItemCommand
            {
                UserId = Guid.NewGuid(),
                Name = "Blue Jeans",
                Category = "Bottoms",
                Color = new string('A', 21),
                Brand = "Levi's",
                Material = "Cotton",
                ImageFront = new byte[] { 1, 2, 3, 4, 5 }
            };

            var frontImageUrl = "https://storage.url/front-image.jpg";
            clothingItemService.UploadImageAsync(
                command.ImageFront,
                command.UserId.ToString(),
                Arg.Any<string>(),
                "ClothingItem",
                "Front")
                .Returns(Result<string>.Success(frontImageUrl));

            embeddingService.GetEmbeddingAsync(Arg.Any<string>())
                .Returns(new float[] { 0.1f, 0.2f, 0.3f });

            clothingItemRepository.AddAsync(Arg.Any<ClothingItem>())
                .Returns(Result<Guid>.Failure("Color cannot exceed 20 characters."));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Color cannot exceed 20 characters.");
        }

        [Fact]
        public async Task Given_EmptyBrand_When_HandlerIsCalled_Then_ShouldFailValidation()
        {
            var command = new CreateClothingItemCommand
            {
                UserId = Guid.NewGuid(),
                Name = "Blue Jeans",
                Category = "Bottoms",
                Color = "Blue",
                Brand = "",
                Material = "Cotton",
                ImageFront = new byte[] { 1, 2, 3, 4, 5 }
            };

            var frontImageUrl = "https://storage.url/front-image.jpg";
            clothingItemService.UploadImageAsync(
                command.ImageFront,
                command.UserId.ToString(),
                Arg.Any<string>(),
                "ClothingItem",
                "Front")
                .Returns(Result<string>.Success(frontImageUrl));

            embeddingService.GetEmbeddingAsync(Arg.Any<string>())
                .Returns(new float[] { 0.1f, 0.2f, 0.3f });

            clothingItemRepository.AddAsync(Arg.Any<ClothingItem>())
                .Returns(Result<Guid>.Failure("Brand is required."));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Brand is required.");
        }

        [Fact]
        public async Task Given_TooLongBrand_When_HandlerIsCalled_Then_ShouldFailValidation()
        {
            var command = new CreateClothingItemCommand
            {
                UserId = Guid.NewGuid(),
                Name = "Blue Jeans",
                Category = "Bottoms",
                Color = "Blue",
                Brand = new string('A', 31),
                Material = "Cotton",
                ImageFront = new byte[] { 1, 2, 3, 4, 5 }
            };

            var frontImageUrl = "https://storage.url/front-image.jpg";
            clothingItemService.UploadImageAsync(
                command.ImageFront,
                command.UserId.ToString(),
                Arg.Any<string>(),
                "ClothingItem",
                "Front")
                .Returns(Result<string>.Success(frontImageUrl));

            embeddingService.GetEmbeddingAsync(Arg.Any<string>())
                .Returns(new float[] { 0.1f, 0.2f, 0.3f });

            clothingItemRepository.AddAsync(Arg.Any<ClothingItem>())
                .Returns(Result<Guid>.Failure("Brand cannot exceed 30 characters."));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Brand cannot exceed 30 characters.");
        }

        [Fact]
        public async Task Given_EmptyMaterial_When_HandlerIsCalled_Then_ShouldFailValidation()
        {
            var command = new CreateClothingItemCommand
            {
                UserId = Guid.NewGuid(),
                Name = "Blue Jeans",
                Category = "Bottoms",
                Color = "Blue",
                Brand = "Levi's",
                Material = "",
                ImageFront = new byte[] { 1, 2, 3, 4, 5 }
            };

            var frontImageUrl = "https://storage.url/front-image.jpg";
            clothingItemService.UploadImageAsync(
                command.ImageFront,
                command.UserId.ToString(),
                Arg.Any<string>(),
                "ClothingItem",
                "Front")
                .Returns(Result<string>.Success(frontImageUrl));

            embeddingService.GetEmbeddingAsync(Arg.Any<string>())
                .Returns(new float[] { 0.1f, 0.2f, 0.3f });

            clothingItemRepository.AddAsync(Arg.Any<ClothingItem>())
                .Returns(Result<Guid>.Failure("Material is required."));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Material is required.");
        }

        [Fact]
        public async Task Given_TooLongMaterial_When_HandlerIsCalled_Then_ShouldFailValidation()
        {
            var command = new CreateClothingItemCommand
            {
                UserId = Guid.NewGuid(),
                Name = "Blue Jeans",
                Category = "Bottoms",
                Color = "Blue",
                Brand = "Levi's",
                Material = new string('A', 31),
                ImageFront = new byte[] { 1, 2, 3, 4, 5 }
            };

            var frontImageUrl = "https://storage.url/front-image.jpg";
            clothingItemService.UploadImageAsync(
                command.ImageFront,
                command.UserId.ToString(),
                Arg.Any<string>(),
                "ClothingItem",
                "Front")
                .Returns(Result<string>.Success(frontImageUrl));

            embeddingService.GetEmbeddingAsync(Arg.Any<string>())
                .Returns(new float[] { 0.1f, 0.2f, 0.3f });

            clothingItemRepository.AddAsync(Arg.Any<ClothingItem>())
                .Returns(Result<Guid>.Failure("Material cannot exceed 30 characters."));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Material cannot exceed 30 characters.");
        }

        [Fact]
        public async Task Given_TooLongPrintType_When_HandlerIsCalled_Then_ShouldFailValidation()
        {
            var command = new CreateClothingItemCommand
            {
                UserId = Guid.NewGuid(),
                Name = "T-Shirt",
                Category = "Tops",
                Color = "White",
                Brand = "Nike",
                Material = "Cotton",
                PrintType = new string('A', 31),
                ImageFront = new byte[] { 1, 2, 3, 4, 5 }
            };

            var frontImageUrl = "https://storage.url/front-image.jpg";
            clothingItemService.UploadImageAsync(
                command.ImageFront,
                command.UserId.ToString(),
                Arg.Any<string>(),
                "ClothingItem",
                "Front")
                .Returns(Result<string>.Success(frontImageUrl));

            embeddingService.GetEmbeddingAsync(Arg.Any<string>())
                .Returns(new float[] { 0.1f, 0.2f, 0.3f });

            clothingItemRepository.AddAsync(Arg.Any<ClothingItem>())
                .Returns(Result<Guid>.Failure("Print type cannot exceed 30 characters."));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Print type cannot exceed 30 characters.");
        }

        [Fact]
        public async Task Given_TooLongPrintDescription_When_HandlerIsCalled_Then_ShouldFailValidation()
        {
            var command = new CreateClothingItemCommand
            {
                UserId = Guid.NewGuid(),
                Name = "T-Shirt",
                Category = "Tops",
                Color = "White",
                Brand = "Nike",
                Material = "Cotton",
                PrintType = "Logo",
                PrintDescription = new string('A', 101),
                ImageFront = new byte[] { 1, 2, 3, 4, 5 }
            };

            var frontImageUrl = "https://storage.url/front-image.jpg";
            clothingItemService.UploadImageAsync(
                command.ImageFront,
                command.UserId.ToString(),
                Arg.Any<string>(),
                "ClothingItem",
                "Front")
                .Returns(Result<string>.Success(frontImageUrl));

            embeddingService.GetEmbeddingAsync(Arg.Any<string>())
                .Returns(new float[] { 0.1f, 0.2f, 0.3f });

            clothingItemRepository.AddAsync(Arg.Any<ClothingItem>())
                .Returns(Result<Guid>.Failure("Print description cannot exceed 100 characters."));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Print description cannot exceed 100 characters.");
        }

        [Fact]
        public async Task Given_TooLongDescription_When_HandlerIsCalled_Then_ShouldFailValidation()
        {
            var command = new CreateClothingItemCommand
            {
                UserId = Guid.NewGuid(),
                Name = "T-Shirt",
                Category = "Tops",
                Color = "White",
                Brand = "Nike",
                Material = "Cotton",
                Description = new string('A', 2001),
                ImageFront = new byte[] { 1, 2, 3, 4, 5 }
            };

            var frontImageUrl = "https://storage.url/front-image.jpg";
            clothingItemService.UploadImageAsync(
                command.ImageFront,
                command.UserId.ToString(),
                Arg.Any<string>(),
                "ClothingItem",
                "Front")
                .Returns(Result<string>.Success(frontImageUrl));

            embeddingService.GetEmbeddingAsync(Arg.Any<string>())
                .Returns(new float[] { 0.1f, 0.2f, 0.3f });

            clothingItemRepository.AddAsync(Arg.Any<ClothingItem>())
                .Returns(Result<Guid>.Failure("Description cannot exceed 2000 characters."));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Description cannot exceed 2000 characters.");
        }

        [Fact]
        public async Task Given_NegativeWeight_When_HandlerIsCalled_Then_ShouldFailValidation()
        {
            var command = new CreateClothingItemCommand
            {
                UserId = Guid.NewGuid(),
                Name = "Blue Jeans",
                Category = "Bottoms",
                Color = "Blue",
                Brand = "Levi's",
                Material = "Cotton",
                ImageFront = new byte[] { 1, 2, 3, 4, 5 },
                Weight = -1.5m
            };

            var frontImageUrl = "https://storage.url/front-image.jpg";
            clothingItemService.UploadImageAsync(
                command.ImageFront,
                command.UserId.ToString(),
                Arg.Any<string>(),
                "ClothingItem",
                "Front")
                .Returns(Result<string>.Success(frontImageUrl));

            embeddingService.GetEmbeddingAsync(Arg.Any<string>())
                .Returns(new float[] { 0.1f, 0.2f, 0.3f });

            clothingItemRepository.AddAsync(Arg.Any<ClothingItem>())
                .Returns(Result<Guid>.Failure("Weight must be a positive value."));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Weight must be a positive value.");
        }

        
    }
}