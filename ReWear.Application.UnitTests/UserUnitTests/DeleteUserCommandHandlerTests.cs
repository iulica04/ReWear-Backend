using Application.Use_Cases.CommandHandlers.UserCommandHandlers;
using Application.Use_Cases.Commands.UserCommands;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace ReWear.Application.UnitTests.UserUnitTests
{
    public class DeleteUserCommandHandlerTests
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        public DeleteUserCommandHandlerTests()
        {
            this.userRepository = Substitute.For<IUserRepository>();
            this.mapper = Substitute.For<IMapper>();
        }

        [Fact]
        public async Task Given_DeleteUserCommandHandler_When_HandlerIsCalled_Then_UserShouldBeDeleted()
        {
            var user = new User
            {
                Id = Guid.Parse("9c922454-33a3-498f-ad9d-d62173cd3bef"),
                FirstName = "Sophia",
                LastName = "Taylor",
                UserName = "sophia.taylor",
                Email = "sophia.taylor@gmail.com",
                PasswordHash = "$2a$11$Vp3mxEdei672TlcjmWTdPel.OHNrHyd746E2nytTgg7rx7Q7pXb0C",
                Role = "User",
                LoginProvider = "Local",
                GoogleId = null,
                ProfilePicture = "https://example.com/profile/sophia.jpg"
            };

            var command = new DeleteUserCommand(user.Id);
            userRepository.GetByIdAsync(user.Id).Returns(user);
            var handler = new DeleteUserCommandHandler(userRepository);

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(Unit.Value);
        }

        [Fact]
        public async Task Given_DeleteUserCommandHandler_When_HandlerIsCalledWithInvalidId_Then_ShouldReturnFailure()
        {
            // Arrange
            var invalidId = new Guid("9c922454-33a3-498f-ad9d-d62173cd3bef");
            userRepository.GetByIdAsync(invalidId).Returns((User?)null);
            var command = new DeleteUserCommand(invalidId);
            var handler = new DeleteUserCommandHandler(userRepository);
            // Act
            var result = await handler.Handle(command, CancellationToken.None);
            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User not found");
        }
    }
}
