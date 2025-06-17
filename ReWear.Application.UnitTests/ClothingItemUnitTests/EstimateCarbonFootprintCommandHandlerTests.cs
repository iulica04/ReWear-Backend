using Application.Models;
using Application.Use_Cases.CommandHandlers.ClothingItemCommandHandlers;
using Application.Use_Cases.Commands.ClothingItemCommands;
using Domain.Entities;
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
    public class EstimateCarbonFootprintCommandHandlerTests
    {
        private readonly IClothingItemRepository repository;
        private readonly EstimateCarbonFootprintCommandHandler handler;

        public EstimateCarbonFootprintCommandHandlerTests()
        {
            repository = Substitute.For<IClothingItemRepository>();
            handler = new EstimateCarbonFootprintCommandHandler(repository);
        }

        [Fact]
        public async Task Handle_ShouldCalculateEmissions_ForKnownMaterialsWithWeight()
        {
            // Arrange
            var userId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

            var items = new List<ClothingItem>
            {
                new ClothingItem
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    UserId = userId,
                    Name = "T-Shirt",
                    Category = "Top",
                    Color = "White",
                    Brand = "BrandA",
                    Material = "Cotton",
                    FrontImageUrl = "url1",
                    Weight = 0.5m,
                    CreatedAt = DateTime.UtcNow,
                    Tags = new(),
                    OutfitClothingItems = new()
                },
                new ClothingItem
                {
                    Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    UserId = userId,
                    Name = "Jacket",
                    Category = "Outerwear",
                    Color = "Black",
                    Brand = "BrandB",
                    Material = "Polyester",
                    FrontImageUrl = "url2",
                    Weight = 0.3m,
                    CreatedAt = DateTime.UtcNow,
                    Tags = new(),
                    OutfitClothingItems = new()
                }
            };

            repository.GetAllAsync().Returns(items);

            var command = new EstimateCarbonFootprintCommand{ UserId = userId };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.TotalCarbonFootprint.Should().BeApproximately((0.5m * 16.4m) + (0.3m * 14.2m), 0.01m);
            result.Data.CountedItems.Should().Be(2);
            result.Data.TotalNumberOfItems.Should().Be(2);
        }

        [Fact]
        public async Task Handle_ShouldSkipItemsWithUnknownMaterials()
        {
            var userId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

            var items = new List<ClothingItem>
            {
                new ClothingItem
                {
                    Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                    UserId = userId,
                    Name = "Alien Shirt",
                    Category = "Top",
                    Color = "Green",
                    Brand = "SpaceBrand",
                    Material = "Unobtainium",
                    FrontImageUrl = "url",
                    Weight = 0.7m,
                    CreatedAt = DateTime.UtcNow,
                    Tags = new(),
                    OutfitClothingItems = new()
                }
            };

            repository.GetAllAsync().Returns(items);

            var command = new EstimateCarbonFootprintCommand{ UserId = userId };
            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Data.TotalCarbonFootprint.Should().Be(0m);
            result.Data.CountedItems.Should().Be(0);
            result.Data.TotalNumberOfItems.Should().Be(1);
        }

        [Fact]
        public async Task Handle_ShouldIgnoreItemsWithNullWeight()
        {
            var userId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

            var items = new List<ClothingItem>
            {
                new ClothingItem
                {
                    Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                    UserId = userId,
                    Name = "Cotton Pants",
                    Category = "Bottom",
                    Color = "Blue",
                    Brand = "EcoBrand",
                    Material = "Cotton",
                    FrontImageUrl = "url",
                    Weight = null,
                    CreatedAt = DateTime.UtcNow,
                    Tags = new(),
                    OutfitClothingItems = new()
                }
            };

            repository.GetAllAsync().Returns(items);

            var command = new EstimateCarbonFootprintCommand{ UserId = userId };
            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Data.TotalCarbonFootprint.Should().Be(0m);
            result.Data.CountedItems.Should().Be(1); // material cunoscut, greutate lipsă
            result.Data.TotalNumberOfItems.Should().Be(1);
        }

        [Fact]
        public async Task Handle_ShouldReturnZero_WhenNoItemsForUser()
        {
            var userId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

            var items = new List<ClothingItem>
            {
                new ClothingItem
                {
                    Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                    UserId = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                    Name = "Other User Item",
                    Category = "Top",
                    Color = "Red",
                    Brand = "BrandX",
                    Material = "Cotton",
                    FrontImageUrl = "url",
                    Weight = 1.0m,
                    CreatedAt = DateTime.UtcNow,
                    Tags = new(),
                    OutfitClothingItems = new()
                }
            };

            repository.GetAllAsync().Returns(items);

            var command = new EstimateCarbonFootprintCommand { UserId = userId};
            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Data.TotalCarbonFootprint.Should().Be(0m);
            result.Data.CountedItems.Should().Be(0);
            result.Data.TotalNumberOfItems.Should().Be(0);
        }
    }
}
