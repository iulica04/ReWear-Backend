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
    public class GetAllUsersQueryHandlerTests
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        public GetAllUsersQueryHandlerTests()
        {
            this.userRepository = Substitute.For<IUserRepository>();
            this.mapper = Substitute.For<IMapper>();

        }
        [Fact]
        public async Task Given_GetAllUsersQueryHandler_When_HandlerIsCalled_Then_AListOfUsersShouldBeReturned()
        {
            //Arrage
            List<User> users = GenerateUsers();
            userRepository.GetAllAsync().Returns(users);
            var query = new GetAllUsersQuery();
            GenerateUsersDto(users);
            var handler = new GetAllUsersQueryHandler(userRepository, mapper);

            //Act
            var result = await handler.Handle(query, CancellationToken.None);

            //Assert
            result.Should().NotBeNull();
            Assert.Equal(2, result.Data.Count);
            Assert.Equal(users[0].Id, result.Data[0].Id);

        }

        private static List<User> GenerateUsers()
        {
            return new List<User>
            {
                new User
                {
                    Id =  Guid.Parse("9c922454-33a3-498f-ad9d-d62173cd3bef"),
                    FirstName = "Sophia",
                    LastName = "Taylor",
                    UserName = "sophia.taylor",
                    Email = "sophia.taylor@gmail.com",
                    PasswordHash = "$2a$11$Vp3mxEdei672TlcjmWTdPel.OHNrHyd746E2nytTgg7rx7Q7pXb0C",
                    Role = "User",
                    LoginProvider = "Local",
                    GoogleId = null,
                    ProfilePicture = "https://example.com/profile/sophia.jpg"
                },
                new User
                {
                    Id = Guid.Parse("3abc6383-9e12-4ca3-8005-f0674a7c28a4"),
                    FirstName = "Ethan",
                    LastName = "Wilson",
                    UserName = "ethan.wilson",
                    Email = "ethan.wilson@yahoo.com",
                    PasswordHash = null,
                    Role = "User",
                    LoginProvider = "Google",
                    GoogleId = "1234567890",
                    ProfilePicture = "https://example.com/profile/ethan.jpg"
                }
            };
        }

        private void GenerateUsersDto(List<User> users)
        {
            mapper.Map<List<UserDTO>>(users).Returns(new List<UserDTO>
            {
                new UserDTO
                {
                    Id = users[0].Id,
                    FirstName = users[0].FirstName,
                    LastName = users[0].LastName,
                    UserName = users[0].UserName,
                    Email = users[0].Email,
                    PasswordHash = users[0].PasswordHash ?? string.Empty,
                    Role = users[0].Role ?? string.Empty,
                    LoginProvider = users[0].LoginProvider,
                    GoogleId = users[0].GoogleId,
                    ProfilePicture = users[0].ProfilePicture ?? string.Empty
                },
                new UserDTO
                {
                    Id = users[1].Id,
                    FirstName = users[1].FirstName,
                    LastName = users[1].LastName,
                    UserName = users[1].UserName,
                    Email = users[1].Email,
                    PasswordHash = users[1].PasswordHash ?? string.Empty,
                    Role = users[1].Role ?? string.Empty,
                    LoginProvider = users[1].LoginProvider,
                    GoogleId = users[1].GoogleId,
                    ProfilePicture = users[1].ProfilePicture ?? string.Empty
                }
            });
        }
    }
}
