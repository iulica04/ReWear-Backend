using Application.DTOs;
using Application.Use_Cases.QueryHandlers.FavoriteOutfitQueryHandlers;
using Application.Use_Cases.Queries.OutfitQueries;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReWear.Application.UnitTests.FavoriteOutfitUnitTests
{
    public class GetUserFavoriteOutfitRecordsQueryHandlerTests
    {
        private readonly IFavoriteOutfitRepository repository;
        private readonly IMapper mapper;
        private readonly GetUserFavoriteOutfitRecordsQueryHandler handler;

        public GetUserFavoriteOutfitRecordsQueryHandlerTests()
        {
            this.repository = Substitute.For<IFavoriteOutfitRepository>();
            this.mapper = Substitute.For<IMapper>();
            this.handler = new GetUserFavoriteOutfitRecordsQueryHandler(repository, mapper);
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedFavoriteOutfits_WhenFavoritesExist()
        {
            // Arrange
            var userId = Guid.Parse("f3f8a848-15c7-4cd2-89dd-c8c545871ac0");
            var favoriteEntities = new List<FavoriteOutfit>
            {
                new FavoriteOutfit
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    OutfitId = Guid.NewGuid()
                }
            };

            var mappedDTOs = new List<FavoriteOutfitDTO>
            {
                new FavoriteOutfitDTO
                {
                    Id = favoriteEntities[0].Id,
                    UserId = favoriteEntities[0].UserId,
                    OutfitId = favoriteEntities[0].OutfitId
                }
            };

            repository.GetAllByUserIdAsync(userId).Returns(favoriteEntities);
            mapper.Map<List<FavoriteOutfitDTO>>(favoriteEntities).Returns(mappedDTOs);

            var query = new GetUserFavoriteOutfitRecordsQuery{ UserId = userId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(mappedDTOs);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoFavoritesExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var emptyFavorites = new List<FavoriteOutfit>();
            var emptyDTOs = new List<FavoriteOutfitDTO>();

            repository.GetAllByUserIdAsync(userId).Returns(emptyFavorites);
            mapper.Map<List<FavoriteOutfitDTO>>(emptyFavorites).Returns(emptyDTOs);

            var query = new GetUserFavoriteOutfitRecordsQuery {  UserId = userId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEmpty();
        }
    }
}
