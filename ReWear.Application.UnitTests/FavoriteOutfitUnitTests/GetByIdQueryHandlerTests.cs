using Application.DTOs;
using Application.Use_Cases.QueryHandlers.FavoriteOutfitQueryHandlers;
using Application.Use_Cases.Queries.FavoriteOutfitQueries;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace ReWear.Application.UnitTests.FavoriteOutfitUnitTests
{
    public class GetByIdQueryHandlerTests
    {
        private readonly IMapper mapper;
        private readonly IFavoriteOutfitRepository repository;
        private readonly GetByIdQueryHandler handler;

        public GetByIdQueryHandlerTests()
        {
            mapper = Substitute.For<IMapper>();
            repository = Substitute.For<IFavoriteOutfitRepository>();
            handler = new GetByIdQueryHandler(mapper, repository);
        }

        [Fact]
        public async Task Handle_ShouldReturnFavoriteOutfitDto_WhenFound()
        {
            // Arrange
            var outfitId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var favoriteOutfit = new FavoriteOutfit
            {
                Id = outfitId,
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                OutfitId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc")
            };

            var expectedDto = new FavoriteOutfitDTO
            {
                Id = favoriteOutfit.Id,
                UserId = favoriteOutfit.UserId,
                OutfitId = favoriteOutfit.OutfitId
            };

            repository.GetByIdAsync(outfitId).Returns(favoriteOutfit);
            mapper.Map<FavoriteOutfitDTO>(favoriteOutfit).Returns(expectedDto);

            var query = new GetByIdQuery { Id = outfitId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(expectedDto);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenNotFound()
        {
            // Arrange
            var outfitId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
            repository.GetByIdAsync(outfitId).Returns((FavoriteOutfit?)null);

            var query = new GetByIdQuery { Id = outfitId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Favorite outfit not found");
        }
    }
}