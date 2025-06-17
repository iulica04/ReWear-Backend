using Application.Use_Cases.CommandHandlers.FavoriteOutfitCommandHandlers;
using Application.Use_Cases.Commands.FavoriteOutfitCommands;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace ReWear.Application.UnitTests.FavoriteOutfitUnitTests
{
    public class DeleteFavoriteOutfitCommandHandlerTests
    {
        private readonly IFavoriteOutfitRepository repository;
        private readonly DeleteFavoriteOutfitCommandHandler handler;

        public DeleteFavoriteOutfitCommandHandlerTests()
        {
            this.repository = Substitute.For<IFavoriteOutfitRepository>();
            this.handler = new DeleteFavoriteOutfitCommandHandler(repository);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenFavoriteIsDeleted()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var outfitId = Guid.NewGuid();
            var favoriteOutfitId = Guid.NewGuid();

            var favoriteOutfit = new FavoriteOutfit
            {
                Id = favoriteOutfitId,
                UserId = userId,
                OutfitId = outfitId
            };

            repository.GetByUserAndOutfitAsync(userId, outfitId).Returns(favoriteOutfit);

            var command = new DeleteFavoriteOutfitCommand
            {
                UserId = userId,
                OutfitId = outfitId
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).DeleteAsync(favoriteOutfitId);
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(Unit.Value);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenFavoriteOutfitNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var outfitId = Guid.NewGuid();

            repository.GetByUserAndOutfitAsync(userId, outfitId).Returns((FavoriteOutfit)null!);

            var command = new DeleteFavoriteOutfitCommand
            {
                UserId = userId,
                OutfitId = outfitId
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Favorite outfit not found");
            await repository.DidNotReceive().DeleteAsync(Arg.Any<Guid>());
        }
    }
}
