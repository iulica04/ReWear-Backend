using Application.Use_Cases.CommandHandlers.FavoriteOutfitCommandHandlers;
using Application.Use_Cases.Commands.FavoriteOutfitCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReWear.Application.UnitTests.FavoriteOutfitUnitTests
{
    public class CreateFavoriteOutfitCommandHandlerTests
    {
        private readonly IFavoriteOutfitRepository repository;
        private readonly IMapper mapper;
        private readonly CreateFavoriteOutfitCommandHandler handler;

        public CreateFavoriteOutfitCommandHandlerTests()
        {
            this.repository = Substitute.For<IFavoriteOutfitRepository>();
            this.mapper = Substitute.For<IMapper>();
            this.handler = new CreateFavoriteOutfitCommandHandler(mapper, repository);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenFavoriteIsCreated()
        {
            // Arrange
            var userId = Guid.Parse("fab59797-78ce-4500-a414-9ab3413e380e");
            var outfitId = Guid.Parse("f6a656c5-ba5b-4062-88c4-3927767580a0");
            var newFavoriteId = Guid.Parse("a3a74e2d-64fd-4a8b-98d2-0bdd3734a50f");

            var command = new CreateFavoriteOutfitCommand
            {
                UserId = userId,
                OutfitId = outfitId
            };

            repository.GetByUserAndOutfitAsync(userId, outfitId).Returns((FavoriteOutfit)null!);
            repository.AddAsync(Arg.Any<FavoriteOutfit>()).Returns(Result<Guid>.Success(newFavoriteId));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(newFavoriteId);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenFavoriteAlreadyExists()
        {
            // Arrange
            var userId = Guid.Parse("fab59797-78ce-4500-a414-9ab3413e380e");
            var outfitId = Guid.Parse("f6a656c5-ba5b-4062-88c4-3927767580a0");

            var command = new CreateFavoriteOutfitCommand
            {
                UserId = userId,
                OutfitId = outfitId
            };

            var existingFavorite = new FavoriteOutfit
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OutfitId = outfitId
            };

            repository.GetByUserAndOutfitAsync(userId, outfitId).Returns(existingFavorite);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Outfit already added to favorites.");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenRepositoryFailsToSave()
        {
            // Arrange
            var userId = Guid.Parse("f6a656c5-ba5b-4062-88c4-3927767580a0");
            var outfitId = Guid.Parse("f6a656c5-ba5b-4062-88c4-3927767580a0");

            var command = new CreateFavoriteOutfitCommand
            {
                UserId = userId,
                OutfitId = outfitId
            };

            repository.GetByUserAndOutfitAsync(userId, outfitId).Returns((FavoriteOutfit)null!);
            repository.AddAsync(Arg.Any<FavoriteOutfit>())
                .Returns(Result<Guid>.Failure("Database insert error"));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Failed to create favorite outfit");
        }
    }
}
