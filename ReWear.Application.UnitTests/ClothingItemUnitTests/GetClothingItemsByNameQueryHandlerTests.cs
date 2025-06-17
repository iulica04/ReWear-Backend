using Application.DTOs;
using Application.Use_Cases.Queries.ClothingItemQueries;
using Application.Use_Cases.QueryHandlers.ClothingItemQueryHandlers;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Models;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReWear.Application.UnitTests.ClothingItemUnitTests
{
    public class GetClothingItemsByNameQueryHandlerTests
    {
        private readonly IClothingItemRepository clothingItemRepository;
        private readonly IMapper mapper;

        public GetClothingItemsByNameQueryHandlerTests()
        {
            clothingItemRepository = Substitute.For<IClothingItemRepository>();
            mapper = Substitute.For<IMapper>();
        }

        [Fact]
        public async Task Given_Handler_When_MatchingItemsExist_Then_ReturnsSuccess()
        {
            // Arrange
            var userId = Guid.Parse("9c922454-33a3-498f-ad9d-d62173cd3bef");

            var matchingItem1 = CreateClothingItem(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), userId, "Blue Jacket");
            var matchingItem2 = CreateClothingItem(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), userId, "Red Jacket");
            var otherUserItem = CreateClothingItem(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Guid.Parse("11111111-1111-1111-1111-111111111111"), "Blue Jacket");

            var allItems = new List<ClothingItem> { matchingItem1, matchingItem2, otherUserItem };
            clothingItemRepository.GetAllAsync().Returns(allItems);

            var expectedDTOs = new List<ClothingItemDTO>
            {
                CreateClothingItemDTO(matchingItem1),
                CreateClothingItemDTO(matchingItem2)
            };

            mapper.Map<List<ClothingItemDTO>>(Arg.Is<List<ClothingItem>>(x => x.Contains(matchingItem1) && x.Contains(matchingItem2)))
                  .Returns(expectedDTOs);

            var handler = new GetClothingItemsByNameQueryHandler(clothingItemRepository, mapper);
            var query = new GetClothingItemsByNameQuery { Name = "jacket", UserId = userId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.Data.Should().BeEquivalentTo(expectedDTOs);
        }

        [Fact]
        public async Task Given_Handler_When_NoMatchingItems_Then_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.Parse("9c922454-33a3-498f-ad9d-d62173cd3bef");
            var greenShirt = CreateClothingItem(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), userId, "Green Shirt");

            clothingItemRepository.GetAllAsync().Returns(new List<ClothingItem> { greenShirt });

            mapper.Map<List<ClothingItemDTO>>(Arg.Any<List<ClothingItem>>()).Returns(new List<ClothingItemDTO>());

            var handler = new GetClothingItemsByNameQueryHandler(clothingItemRepository, mapper);
            var query = new GetClothingItemsByNameQuery { Name = "jacket", UserId = userId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEmpty();
        }

        #region Helper Methods

        private static ClothingItem CreateClothingItem(Guid id, Guid userId, string name)
        {
            return new ClothingItem
            {
                Id = id,
                UserId = userId,
                Name = name,
                Category = "Outerwear",
                Color = "Blue",
                Brand = "ReWear",
                Material = "Cotton",
                Description = "A stylish jacket",
                PrintDescription = "None",
                PrintType = "None",
                FrontImageUrl = "https://example.com/front.jpg",
                BackImageUrl = "https://example.com/back.jpg",
                NumberOfWears = 3,
                LastWornDate = DateTime.UtcNow.AddDays(-40),
                Tags = new List<ClothingTag>
                {
                    new ClothingTag { Tag = "Winter" },
                    new ClothingTag { Tag = "Stylish" }
                }
            };
        }

        private static ClothingItemDTO CreateClothingItemDTO(ClothingItem item)
        {
            return new ClothingItemDTO
            {
                Id = item.Id,
                UserId = item.UserId,
                Name = item.Name,
                Category = item.Category,
                Tags = item.Tags.Select(t => t.Tag).ToList(),
                Color = item.Color,
                Brand = item.Brand,
                Material = item.Material,
                Description = item.Description,
                PrintDescription = item.PrintDescription,
                PrintType = item.PrintType,
                FrontImageUrl = item.FrontImageUrl,
                BackImageUrl = item.BackImageUrl,
                NumberOfWears = item.NumberOfWears,
                LastWornDate = item.LastWornDate
            };
        }

        #endregion
    }
}
