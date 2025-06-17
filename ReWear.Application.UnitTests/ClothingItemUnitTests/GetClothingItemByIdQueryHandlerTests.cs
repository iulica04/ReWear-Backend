using Application.DTOs;
using Application.Use_Cases.Queries.ClothingItemQueries;
using Application.Use_Cases.QueryHandlers.ClothingItemQueryHandlers;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReWear.Application.UnitTests.ClothingItemUnitTests
{
    public class GetClothingItemByIdQueryHandlerTests
    {
        private readonly IClothingItemRepository clothingItemRepository;
        private readonly IMapper mapper;

        public GetClothingItemByIdQueryHandlerTests()
        {
            this.clothingItemRepository = Substitute.For<IClothingItemRepository>();
            this.mapper = Substitute.For<IMapper>();
        }

        [Fact]
        public async Task Given_GetClothingItemByIdQueryHandler_When_ItemExists_Then_ReturnsClothingItemDto()
        {
            // Arrange
            var clothingItem = GenerateClothingItem();
            clothingItemRepository.GetByIdAsync(clothingItem.Id).Returns(clothingItem);
            GenerateClothingItemDto(clothingItem);

            var query = new GetClothingItemByIdQuery { Id = clothingItem.Id };
            var handler = new GetClothingItemByIdQueryHandler(clothingItemRepository, mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Id.Should().Be(clothingItem.Id);
            result.Data.Name.Should().Be(clothingItem.Name);
            result.Data.Category.Should().Be(clothingItem.Category);
            result.Data.Color.Should().Be(clothingItem.Color);
            result.Data.Brand.Should().Be(clothingItem.Brand);
            result.Data.Material.Should().Be(clothingItem.Material);
            result.Data.FrontImageUrl.Should().Be(clothingItem.FrontImageUrl);
            result.Data.BackImageUrl.Should().Be(clothingItem.BackImageUrl);
        }

        [Fact]
        public async Task Given_GetClothingItemByIdQueryHandler_When_ItemDoesNotExist_Then_ReturnsFailureResult()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            clothingItemRepository.GetByIdAsync(invalidId).Returns((ClothingItem?)null);
            var query = new GetClothingItemByIdQuery { Id = invalidId };
            var handler = new GetClothingItemByIdQueryHandler(clothingItemRepository, mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Clothing item not found");
        }

        private void GenerateClothingItemDto(ClothingItem clothingItem)
        {
            mapper.Map<ClothingItemDTO>(clothingItem).Returns(new ClothingItemDTO
            {
                Id = clothingItem.Id,
                Name = clothingItem.Name,
                Category = clothingItem.Category,
                Color = clothingItem.Color,
                Brand = clothingItem.Brand,
                Material = clothingItem.Material,
                FrontImageUrl = clothingItem.FrontImageUrl,
                BackImageUrl = clothingItem.BackImageUrl!
            });
        }

        private static ClothingItem GenerateClothingItem()
        {
            return new ClothingItem
            {
                Id = Guid.Parse("3f52d0f4-0c43-4a6a-8e22-d8a7e87bcf91"),
                Name = "Winter Jacket",
                Category = "Outerwear",
                Color = "Blue",
                Brand = "WinterCo",
                Material = "Polyester",
                FrontImageUrl = "https://example.com/images/winter-jacket-front.jpg",
                BackImageUrl = "https://example.com/images/winter-jacket-back.jpg"
            };
        }
    }
}
