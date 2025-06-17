using Application.DTOs;
using Application.Use_Cases.Queries;
using Application.Use_Cases.Queries.ClothingItemQueries;
using AutoMapper;
using Domain.Entities;
using Domain.Models;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReWear.Application.UnitTests.ClothingItemUnitTests
{
    public class GetFilteredClothingItemsQueryHandlerTests
    {
        private readonly IClothingItemRepository clothingItemRepository;
        private readonly IMapper mapper;

        public GetFilteredClothingItemsQueryHandlerTests()
        {
            clothingItemRepository = Substitute.For<IClothingItemRepository>();
            mapper = Substitute.For<IMapper>();
        }

        [Fact]
        public async Task Given_ValidFilterQuery_When_HandlerIsCalled_Then_FilteredAndPagedResultsShouldBeReturned()
        {
            // Arrange
            var allItems = GenerateClothingItems();
            clothingItemRepository.GetAllAsync().Returns(allItems);

            // Filter: only items with Brand == "Levi's"
            Expression<Func<ClothingItem, bool>> filter = item => item.Brand == "Levi's";

            var query = new GetFilteredQuery<ClothingItem, ClothingItemDTO>
            {
                Page = 1,
                PageSize = 10,
                Filter = filter
            };

            var expectedFilteredItems = allItems.Where(filter.Compile()).ToList();

            mapper.Map<List<ClothingItemDTO>>(Arg.Any<List<ClothingItem>>())
                .Returns(callInfo =>
                    ((List<ClothingItem>)callInfo[0])
                        .Select(i => new ClothingItemDTO
                        {
                            Id = i.Id,
                            UserId = i.UserId,
                            Name = i.Name,
                            Category = i.Category,
                            Color = i.Color,
                            Brand = i.Brand,
                            Material = i.Material,
                            PrintType = i.PrintType,
                            PrintDescription = i.PrintDescription,
                            Description = i.Description,
                            FrontImageUrl = i.FrontImageUrl,
                            BackImageUrl = i.BackImageUrl,
                            NumberOfWears = i.NumberOfWears,
                            LastWornDate = i.LastWornDate,

                            // Tags and OutfitClothingItems can be mapped separately if needed
                        })
                        .ToList()
                );

            var handler = new GetFilteredClothingItemsQueryHandler(clothingItemRepository, mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.TotalCount.Should().Be(expectedFilteredItems.Count);
            result.Data.Data.Should().AllSatisfy(dto => dto.Brand.Should().Be("Levi's"));
        }

        private List<ClothingItem> GenerateClothingItems()
        {
            return new List<ClothingItem>
            {
                new ClothingItem
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Name = "Blue Jeans",
                    Category = "Pants",
                    Tags = new List<ClothingTag>(), // Can add some tags if needed
                    Color = "Blue",
                    Brand = "Levi's",
                    Material = "Denim",
                    PrintType = null,
                    PrintDescription = null,
                    Description = "Classic blue jeans",
                    FrontImageUrl = "https://example.com/images/blue_jeans_front.jpg",
                    BackImageUrl = "https://example.com/images/blue_jeans_back.jpg",
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    NumberOfWears = 5,
                    LastWornDate = DateTime.UtcNow.AddDays(-1),
                    Weight = 0.7m,
                    IsSold = false
                },
                new ClothingItem
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Name = "Red T-Shirt",
                    Category = "Shirts",
                    Tags = new List<ClothingTag>(),
                    Color = "Red",
                    Brand = "H&M",
                    Material = "Cotton",
                    PrintType = "Screen Print",
                    PrintDescription = "Logo on front",
                    Description = "Comfortable red T-shirt",
                    FrontImageUrl = "https://example.com/images/red_tshirt_front.jpg",
                    BackImageUrl = null,
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    NumberOfWears = 3,
                    LastWornDate = DateTime.UtcNow.AddDays(-2),
                    Weight = 0.3m,
                    IsSold = false
                },
                new ClothingItem
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Name = "Black Jacket",
                    Category = "Jackets",
                    Tags = new List<ClothingTag>(),
                    Color = "Black",
                    Brand = "Levi's",
                    Material = "Leather",
                    PrintType = null,
                    PrintDescription = null,
                    Description = "Stylish black leather jacket",
                    FrontImageUrl = "https://example.com/images/black_jacket_front.jpg",
                    BackImageUrl = null,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    NumberOfWears = 7,
                    LastWornDate = DateTime.UtcNow.AddDays(-3),
                    Weight = 1.2m,
                    IsSold = true
                }
            };
        }
    }
}
