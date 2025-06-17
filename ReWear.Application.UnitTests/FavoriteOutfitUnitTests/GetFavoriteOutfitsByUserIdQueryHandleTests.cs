using Application.DTOs;
using Application.Use_Cases.QueryHandlers.FavoriteOutfitQueryHandlers;
using Application.Use_Cases.Queries.FavoriteOutfitQueries;
using Application.Utils;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReWear.Application.UnitTests.FavoriteOutfitUnitTests
{
    public class GetFavoriteOutfitsByUserIdQueryHandlerTests
    {
        private readonly IFavoriteOutfitRepository favoriteOutfitRepository;
        private readonly IOutfitRepository outfitRepository;
        private readonly GetFavoriteOutfitsByUserIdQueryHandler handler;

        public GetFavoriteOutfitsByUserIdQueryHandlerTests()
        {
            this.favoriteOutfitRepository = Substitute.For<IFavoriteOutfitRepository>();
            this.outfitRepository = Substitute.For<IOutfitRepository>();
            this.handler = new GetFavoriteOutfitsByUserIdQueryHandler(favoriteOutfitRepository, outfitRepository);
        }

        [Fact]
        public async Task Handle_ShouldReturnPagedResult_WhenFavoritesExist()
        {
            // Arrange
            var userId = Guid.Parse("a3a74e2d-64fd-4a8b-98d2-0bdd3734a50f");
            var outfitId = Guid.Parse("fb556868-d477-4161-8058-ff0d9e1d7d25");

            var favoriteOutfits = new List<FavoriteOutfit>
            {
                new FavoriteOutfit { Id = Guid.NewGuid(), UserId = userId, OutfitId = outfitId }
            };

            var clothingItem = new ClothingItem
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Shirt",
                Category = "Top",
                Color = "Red",
                Brand = "BrandX",
                Material = "Cotton",
                PrintType = "None",
                PrintDescription = "Plain",
                Description = "Red shirt",
                FrontImageUrl = "front.jpg",
                BackImageUrl = "back.jpg",
                NumberOfWears = 3
            };

            var outfit = new Outfit
            {
                Id = outfitId,
                UserId = userId,
                Name = "Casual Outfit",
                CreatedAt = DateTime.UtcNow,
                Season = "Summer",
                Description = "Summer casual",
                ImageUrl = "outfit.jpg",
                OutfitClothingItems = new List<OutfitClothingItem>
                {
                    new OutfitClothingItem
                    {
                        ClothingItem = clothingItem
                    }
                }
            };

            favoriteOutfitRepository.GetAllByUserIdAsync(userId).Returns(favoriteOutfits);
            outfitRepository.GetByIdAsync(outfitId).Returns(outfit);

            var query = new GetFavoriteOutfitsByUserIdQuery { UserId = userId, Page = 1, PageSize = 10 };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Data.Should().HaveCount(1);
            result.Data.TotalCount.Should().Be(1);

            var returnedOutfit = result.Data.Data.First();
            returnedOutfit.Id.Should().Be(outfit.Id);
            returnedOutfit.ClothingItemDTOs.Should().HaveCount(1);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyResult_WhenNoFavoritesExist()
        {
            // Arrange
            var userId = Guid.Parse("f3f8a848-15c7-4cd2-89dd-c8c545871ac0");

            favoriteOutfitRepository.GetAllByUserIdAsync(userId).Returns(new List<FavoriteOutfit>());

            var query = new GetFavoriteOutfitsByUserIdQuery{ UserId = userId, Page = 1, PageSize = 10 };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Data.Should().BeEmpty();
            result.Data.TotalCount.Should().Be(0);
        }

        [Fact]
        public async Task Handle_ShouldSkipNullOutfits()
        {
            // Arrange
            var userId = Guid.Parse("f3f8a848-15c7-4cd2-89dd-c8c545871ac0");
            var favoriteOutfits = new List<FavoriteOutfit>
            {
                new FavoriteOutfit { Id = Guid.NewGuid(), UserId = userId, OutfitId = Guid.NewGuid() },
                new FavoriteOutfit { Id = Guid.NewGuid(), UserId = userId, OutfitId = Guid.NewGuid() }
            };

            favoriteOutfitRepository.GetAllByUserIdAsync(userId).Returns(favoriteOutfits);
            outfitRepository.GetByIdAsync(Arg.Any<Guid>()).Returns((Outfit?)null);

            var query = new GetFavoriteOutfitsByUserIdQuery {UserId = userId, Page = 1, PageSize = 10 };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Data.Should().BeEmpty();
            result.Data.TotalCount.Should().Be(0);
        }
    }
}
