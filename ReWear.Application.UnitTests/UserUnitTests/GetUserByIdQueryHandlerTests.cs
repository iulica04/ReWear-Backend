using Application.DTOs;
using Application.Use_Cases.Queries.UserQueries;
using Application.Use_Cases.QueryHandlers.UserQueryHandlers;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace ReWear.Application.UnitTests.UserUnitTests
{
    public class GetUserByIdQueryHandlerTests
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        public GetUserByIdQueryHandlerTests()
        {
            this.userRepository = Substitute.For<IUserRepository>();
            this.mapper = Substitute.For<IMapper>();
        }

        [Fact]
        public async Task Given_GetUserByIdQueryHandlerTests_When_HandlerIsCalled_Then_AUserShuldBeReturned()
        {
            // Arrange
            var user = GenerateUser();
            userRepository.GetByIdAsync(user.Id).Returns(user);
            var query = new GetUserByIdQuery { Id = user.Id };
            GenerateUserDto(user);
            var handler = new GetUserByIdQueryHandler(userRepository, mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Data.Id.Should().Be(user.Id);
            result.Data.FirstName.Should().Be(user.FirstName);
            result.Data.LastName.Should().Be(user.LastName);
            result.Data.UserName.Should().Be(user.UserName);
            result.Data.Email.Should().Be(user.Email);
            result.Data.PasswordHash.Should().Be(user.PasswordHash);
            result.Data.Role.Should().Be(user.Role);
            result.Data.LoginProvider.Should().Be(user.LoginProvider);
            result.Data.GoogleId.Should().Be(user.GoogleId);
            result.Data.ProfilePicture.Should().Be(user.ProfilePicture);
        }

        [Fact]
        public async Task Given_GetUserByIdQueryHandlerTests_When_HandlerIsCalledWithInvalidId_Then_ShouldThrowException()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            userRepository.GetByIdAsync(invalidId).Returns((User?)null);
            var query = new GetUserByIdQuery { Id = invalidId };
            var handler = new GetUserByIdQueryHandler(userRepository, mapper);


            // Act
            var result = await handler.Handle(query, CancellationToken.None);


            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User not found");
        }

        private void GenerateUserDto(User user)
        {
            mapper.Map<UserDTO>(user).Returns(new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                PasswordHash = user.PasswordHash ?? string.Empty,
                Role = user.Role ?? string.Empty,
                ProfilePicture = user.ProfilePicture ?? string.Empty,
                GoogleId = user.GoogleId,
                LoginProvider = user.LoginProvider
            });
        }

        private static User GenerateUser()
        {
            return new User
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
        }
    }
}
