using Application.Use_Cases.CommandHandlers.OutfitCommandHandlers;
using Application.Use_Cases.Commands.OutfitCommands;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReWear.Application.UnitTests.OutfitUnitTests
{
    public class DeleteOutfitCommandHandlerTests
    {
        private readonly IOutfitRepository repository;
        private readonly DeleteOutfitCommandHandler handler;

        public DeleteOutfitCommandHandlerTests()
        {
            repository = Substitute.For<IOutfitRepository>();
            handler = new DeleteOutfitCommandHandler(repository);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenOutfitExists()
        {
            // Arrange
            var id = Guid.Parse("8c1ae239-734b-4d8d-891d-5e7fd40ea662");
            var outfit = new Outfit { Id = id, UserId = id, Name = "Test", ImageUrl = "url" };
            repository.GetByIdAsync(id).Returns(outfit);

            var command = new DeleteOutfitCommand (id);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNull();
            await repository.Received(1).DeleteAsync(id);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenOutfitDoesNotExist()
        {
            // Arrange
            var id = Guid.Parse("8c1ae239-734b-4d8d-891d-5e7fd40ea662");
            repository.GetByIdAsync(id).Returns((Outfit)null);

            var command = new DeleteOutfitCommand(id);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Outfit not found");
            await repository.DidNotReceive().DeleteAsync(id);
        }
    }
}