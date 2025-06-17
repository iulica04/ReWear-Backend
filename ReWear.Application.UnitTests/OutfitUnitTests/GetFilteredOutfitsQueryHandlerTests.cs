using Application.DTOs;
using Application.Use_Cases.QueryHandlers.OutfitQueryHandlers;
using Application.Use_Cases.Queries;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
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

namespace ReWear.Application.UnitTests.OutfitUnitTests
{
    public class GetFilteredOutfitsQueryHandlerTests
    {
        private readonly IOutfitRepository outfitRepository = Substitute.For<IOutfitRepository>();
        private readonly IMapper mapper = Substitute.For<IMapper>();
        private readonly GetFilteredOutfitsQueryHandler handler;

        public GetFilteredOutfitsQueryHandlerTests()
        {
            handler = new GetFilteredOutfitsQueryHandler(outfitRepository, mapper);
        }

        private static List<Outfit> GetFakeOutfits()
        {
            return new List<Outfit>
            {
                new Outfit
                {
                    Id = Guid.Parse("828a3bab-28be-4b31-b5d6-c81439250e63"),
                    UserId = Guid.Parse("29555045-570f-4821-be10-c149f44009cd"),
                    Name = "Summer Look",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    Season = "Summer",
                    Description = "For hot days",
                    ImageUrl = "summer.jpg",
                    OutfitClothingItems = new List<OutfitClothingItem>
                    {
                        new OutfitClothingItem
                        {
                            ClothingItem = new ClothingItem
                            {
                                Id = Guid.Parse("8130ef37-25b4-4633-9350-3e8de9e0f3fd"),
                                UserId = Guid.Parse("8130ef37-25b4-4633-9350-3e8de9e0f3fd"), // FIXED: valid GUID
                                Name = "T-Shirt",
                                Category = "Top",
                                Color = "White",
                                Brand = "Zara",
                                Material = "Cotton",
                                PrintType = "None",
                                PrintDescription = null,
                                Description = "Basic white tee",
                                FrontImageUrl = "front.jpg",
                                BackImageUrl = "back.jpg",
                                NumberOfWears = 10,
                                LastWornDate = DateTime.Today.AddDays(-1)
                            }
                        }
                    }
                },
                new Outfit
                {
                    Id = Guid.Parse("29555045-570f-4821-be10-c149f44009cd"),
                    UserId = Guid.Parse("29555045-570f-4821-be10-c149f44009cd"),
                    Name = "Winter Look",
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    Season = "Winter",
                    Description = "For cold days",
                    ImageUrl = "winter.jpg",
                    OutfitClothingItems = new List<OutfitClothingItem>()
                }
            };
        }

        [Fact]
        public async Task Handle_ShouldReturnPagedResult_WhenNoFilterIsApplied()
        {
            // Arrange
            var outfits = GetFakeOutfits();
            outfitRepository.GetAllAsync().Returns(outfits);

            var request = new GetFilteredQuery<Outfit, OutfitDTO>
            {
                Filter = null,
                Page = 1,
                PageSize = 10
            };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.TotalCount.Should().Be(2);
            result.Data.Data.Should().HaveCount(2);
        }

        [Fact]
        public async Task Handle_ShouldReturnFilteredResults_WhenFilterIsApplied()
        {
            // Arrange
            var outfits = GetFakeOutfits();
            outfitRepository.GetAllAsync().Returns(outfits);

            var request = new GetFilteredQuery<Outfit, OutfitDTO>
            {
                Filter = o => o.Season == "Winter",
                Page = 1,
                PageSize = 10
            };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Data.Should().HaveCount(1);
            result.Data.Data.First().Season.Should().Be("Winter");
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenFilterDoesNotMatchAnyOutfit()
        {
            // Arrange
            var outfits = GetFakeOutfits();
            outfitRepository.GetAllAsync().Returns(outfits);

            var request = new GetFilteredQuery<Outfit, OutfitDTO>
            {
                Filter = o => o.Season == "Spring", // No Spring outfit in fake data
                Page = 1,
                PageSize = 10
            };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Data.Should().BeEmpty();
            result.Data.TotalCount.Should().Be(0);
        }

        [Fact]
        public async Task Handle_ShouldMapClothingItemsCorrectly()
        {
            // Arrange
            var outfits = GetFakeOutfits();
            outfitRepository.GetAllAsync().Returns(outfits);

            var request = new GetFilteredQuery<Outfit, OutfitDTO>
            {
                Filter = null,
                Page = 1,
                PageSize = 10
            };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            var firstOutfit = result.Data.Data.First(o => o.Name == "Summer Look");
            firstOutfit.ClothingItemDTOs.Should().HaveCount(1);
            firstOutfit.ClothingItemDTOs.First().Name.Should().Be("T-Shirt");
        }
    }
}