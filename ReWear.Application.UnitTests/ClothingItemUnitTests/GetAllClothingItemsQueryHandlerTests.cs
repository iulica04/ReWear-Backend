using Application.DTOs;
using Application.Use_Cases.Queries.ClothingItemQueries;
using Application.Use_Cases.QueryHandlers.ClothingItemQueryHandlers;
using AutoMapper;
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

namespace ReWear.Application.UnitTests.ClothingItemUnitTests
{
    public class GetAllClothingItemsQueryHandlerTests
    {
        private readonly IClothingItemRepository clothingItemRepository;
        private readonly IMapper mapper;

        public GetAllClothingItemsQueryHandlerTests()
        {
            this.clothingItemRepository = Substitute.For<IClothingItemRepository>();
            this.mapper = Substitute.For<IMapper>();
        }

        [Fact]
        public async Task Given_GetAllClothingItemsQueryHandler_When_HandlerIsCalled_Then_AListOfClothingItemsShouldBeReturned()
        {
            // Arrange
            List<ClothingItem> clothingItems = GenerateClothingItems();
            clothingItemRepository.GetAllAsync().Returns(clothingItems);

            var query = new GetAllClothingItemsQuery();

            // Setup the mapper to convert domain entities to DTOs
            mapper.Map<List<ClothingItemDTO>>(clothingItems).Returns(GenerateClothingItemDTOs(clothingItems));

            var handler = new GetAllClothingItemsQueryHandler(clothingItemRepository, mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.Data[0].Id.Should().Be(clothingItems[0].Id);
            result.Data[1].Name.Should().Be(clothingItems[1].Name);
        }

        private static List<ClothingItem> GenerateClothingItems()
        {
            return new List<ClothingItem>
            {
                new ClothingItem
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Name = "Blue Jeans",
                    Category = "Pants",
                    Tags = new List<ClothingTag> { new ClothingTag {Tag = "Denim" } },
                    Color = "Blue",
                    Brand = "Levi's",
                    Material = "Denim",
                    FrontImageUrl = "https://example.com/bluejeans-front.jpg",
                    BackImageUrl = "https://example.com/bluejeans-back.jpg",
                    NumberOfWears = 10,
                    LastWornDate = DateTime.UtcNow.AddDays(-3)
                },
                new ClothingItem
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    Name = "Red T-Shirt",
                    Category = "Shirts",
                    Tags = new List<ClothingTag> { new ClothingTag { Tag = "Cotton" } },
                    Color = "Red",
                    Brand = "H&M",
                    Material = "Cotton",
                    FrontImageUrl = "https://example.com/redtshirt-front.jpg",
                    BackImageUrl = "https://example.com/redtshirt-back.jpg",
                    NumberOfWears = 5,
                    LastWornDate = DateTime.UtcNow.AddDays(-1)
                }
            };
        }

        private static List<ClothingItemDTO> GenerateClothingItemDTOs(List<ClothingItem> items)
        {
            return new List<ClothingItemDTO>
            {
                new ClothingItemDTO
                {
                    Id = items[0].Id,
                    UserId = items[0].UserId,
                    Name = items[0].Name,
                    Category = items[0].Category,
                    Tags = new List<string> { "Denim" },
                    Color = items[0].Color,
                    Brand = items[0].Brand,
                    Material = items[0].Material,
                    FrontImageUrl = items[0].FrontImageUrl,
                    BackImageUrl = items[0].BackImageUrl ?? string.Empty,
                    NumberOfWears = items[0].NumberOfWears,
                    LastWornDate = items[0].LastWornDate,
                    PrintType = items[0].PrintType,
                    PrintDescription = items[0].PrintDescription,
                    Description = items[0].Description
                },
                new ClothingItemDTO
                {
                    Id = items[1].Id,
                    UserId = items[1].UserId,
                    Name = items[1].Name,
                    Category = items[1].Category,
                    Tags = new List<string> { "Cotton" },
                    Color = items[1].Color,
                    Brand = items[1].Brand,
                    Material = items[1].Material,
                    FrontImageUrl = items[1].FrontImageUrl,
                    BackImageUrl = items[1].BackImageUrl ?? string.Empty,
                    NumberOfWears = items[1].NumberOfWears,
                    LastWornDate = items[1].LastWornDate,
                    PrintType = items[1].PrintType,
                    PrintDescription = items[1].PrintDescription,
                    Description = items[1].Description
                }
            };
        }
    }
}
