using Application.Services;
using Application.Use_Cases.CommandHandlers;
using Application.Use_Cases.Commands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace ReWear.Application.UnitTests.UserUnitTests
{
    public class CreateUserCommandHandlerTests
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly CreateUserCommandHandler handler;
        private readonly IPasswordHasher passwordHasher;

        public CreateUserCommandHandlerTests()
        {
            this.userRepository = Substitute.For<IUserRepository>();
            this.mapper = Substitute.For<IMapper>();
            this.passwordHasher = Substitute.For<IPasswordHasher>();
            this.handler = new CreateUserCommandHandler(userRepository, mapper, passwordHasher);
        }

        [Fact]
        public async Task Given_CreateUserCommandHandler_When_HandlerISCalled_Then_UserShouldBeCreated()
        {
            var command = new CreateUserCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garci@gamil.com",
                Password = "Password123!",
            };

            var user = new User
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                PasswordHash = passwordHasher.HashPassword(command.Password),
                Role = "User",
                LoginProvider = "Local",
            };
            mapper.Map<User>(command).Returns(user);
            userRepository.AddAsync(user).Returns(Result<Guid>.Success(user.Id));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await userRepository.Received(1).AddAsync(user);
            result.Data.Should().Be(user.Id);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Given_EmptyFirstNameForCreateUserCommand_When_HandlerISCalled_Then_UserShouldNotBeCreated()
        {
            var command = new CreateUserCommand
            {
                FirstName = "",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garci@gamil.com",
                Password = "Password123!",
            };

            var user = new User
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                PasswordHash = passwordHasher.HashPassword(command.Password),
                Role = "User",
                LoginProvider = "Local",
            };
            mapper.Map<User>(command).Returns(user);
            userRepository.AddAsync(user).Returns(Result<Guid>.Failure("First name is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await userRepository.Received(1).AddAsync(user);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("First name is required.");
        }


        [Fact]
        public async Task Given_FirstNameGreaterThan30CharactersForCreateUserCommand_When_HandlerISCalled_Then_UserShouldNotBeCreated()
        {
            var command = new CreateUserCommand
            {
                FirstName = "ChristopherAlexanderJohnsonWilliams",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garci@gamil.com",
                Password = "Password123!",
            };

            var user = new User
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                PasswordHash = passwordHasher.HashPassword(command.Password),
                Role = "User",
                LoginProvider = "Local",
            };
            mapper.Map<User>(command).Returns(user);
            userRepository.AddAsync(user).Returns(Result<Guid>.Failure("First name must be between 2 and 30 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await userRepository.Received(1).AddAsync(user);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("First name must be between 2 and 30 characters.");
        }

        [Fact]
        public async Task Given_FirstNameLowerThan30CharactersForCreateUserCommand_When_HandlerISCalled_Then_UserShouldNotBeCreated()
        {
            var command = new CreateUserCommand
            {
                FirstName = "C",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garci@gamil.com",
                Password = "Password123!",
            };

            var user = new User
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                PasswordHash = passwordHasher.HashPassword(command.Password),
                Role = "User",
                LoginProvider = "Local",
            };
            mapper.Map<User>(command).Returns(user);
            userRepository.AddAsync(user).Returns(Result<Guid>.Failure("First name must be between 2 and 30 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await userRepository.Received(1).AddAsync(user);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("First name must be between 2 and 30 characters.");
        }

        [Fact]
        public async Task Given_EmptyLastNameForCreateUserCommand_When_HandlerISCalled_Then_UserShouldNotBeCreated()
        {
            var command = new CreateUserCommand
            {
                FirstName = "Ethan",
                LastName = "",
                UserName = "ethan.garcia",
                Email = "ethan.garci@gamil.com",
                Password = "Password123!",
            };

            var user = new User
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                PasswordHash = passwordHasher.HashPassword(command.Password),
                Role = "User",
                LoginProvider = "Local",
            };
            mapper.Map<User>(command).Returns(user);
            userRepository.AddAsync(user).Returns(Result<Guid>.Failure("Last name is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await userRepository.Received(1).AddAsync(user);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Last name is required.");
        }


        [Fact]
        public async Task Given_LastNameGreaterThan30CharactersForCreateUserCommand_When_HandlerISCalled_Then_UserShouldNotBeCreated()
        {
            var command = new CreateUserCommand
            {
                FirstName = "Ethan",
                LastName = "ChristopherAlexanderJohnsonWilliams",
                UserName = "ethan.garcia",
                Email = "ethan.garci@gamil.com",
                Password = "Password123!",
            };

            var user = new User
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                PasswordHash = passwordHasher.HashPassword(command.Password),
                Role = "User",
                LoginProvider = "Local",
            };
            mapper.Map<User>(command).Returns(user);
            userRepository.AddAsync(user).Returns(Result<Guid>.Failure("Last name must be between 2 and 30 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await userRepository.Received(1).AddAsync(user);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Last name must be between 2 and 30 characters.");
        }

        [Fact]
        public async Task Given_LastNameLowerThan30CharactersForCreateUserCommand_When_HandlerISCalled_Then_UserShouldNotBeCreated()
        {
            var command = new CreateUserCommand
            {
                FirstName = "Ethan",
                LastName = "G",
                UserName = "ethan.garcia",
                Email = "ethan.garci@gamil.com",
                Password = "Password123!",
            };

            var user = new User
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                PasswordHash = passwordHasher.HashPassword(command.Password),
                Role = "User",
                LoginProvider = "Local",
            };
            mapper.Map<User>(command).Returns(user);
            userRepository.AddAsync(user).Returns(Result<Guid>.Failure("Last name must be between 2 and 30 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await userRepository.Received(1).AddAsync(user);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Last name must be between 2 and 30 characters.");
        }

        [Fact]
        public async Task Given_EmptyUserNameForCreateUserCommand_When_HandlerISCalled_Then_UserShouldNotBeCreated()
        {
            var command = new CreateUserCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "",
                Email = "ethan.garci@gamil.com",
                Password = "Password123!",
            };

            var user = new User
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                PasswordHash = passwordHasher.HashPassword(command.Password),
                Role = "User",
                LoginProvider = "Local",
            };
            mapper.Map<User>(command).Returns(user);
            userRepository.AddAsync(user).Returns(Result<Guid>.Failure("User name is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await userRepository.Received(1).AddAsync(user);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User name is required.");
        }

        [Fact]
        public async Task Given_UserNameGreaterThan30CharactersForCreateUserCommand_When_HandlerISCalled_Then_UserShouldNotBeCreated()
        {
            var command = new CreateUserCommand
            {
                FirstName = "Ethan",
                LastName = "Christopher",
                UserName = "ChristopherAlexanderJohnsonWilliams.garcia",
                Email = "ethan.garci@gamil.com",
                Password = "Password123!",
            };

            var user = new User
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                PasswordHash = passwordHasher.HashPassword(command.Password),
                Role = "User",
                LoginProvider = "Local",
            };
            mapper.Map<User>(command).Returns(user);
            userRepository.AddAsync(user).Returns(Result<Guid>.Failure("User name must be between 2 and 30 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await userRepository.Received(1).AddAsync(user);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User name must be between 2 and 30 characters.");
        }

        [Fact]
        public async Task Given_UserNameLowerThan30CharactersForCreateUserCommand_When_HandlerISCalled_Then_UserShouldNotBeCreated()
        {
            var command = new CreateUserCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "e",
                Email = "ethan.garci@gamil.com",
                Password = "Password123!",
            };

            var user = new User
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                PasswordHash = passwordHasher.HashPassword(command.Password),
                Role = "User",
                LoginProvider = "Local",
            };
            mapper.Map<User>(command).Returns(user);
            userRepository.AddAsync(user).Returns(Result<Guid>.Failure("User name must be between 2 and 30 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await userRepository.Received(1).AddAsync(user);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User name must be between 2 and 30 characters.");
        }

        [Fact]
        public async Task Given_ExistingUserNameForCreateUserCommand_When_HandlerISCalled_Then_UserShouldNotBeCreated()
        {
            var command = new CreateUserCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garci@gamil.com",
                Password = "Password123!",
            };

            var user = new User
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                PasswordHash = passwordHasher.HashPassword(command.Password),
                Role = "User",
                LoginProvider = "Local",
            };
            mapper.Map<User>(command).Returns(user);
            userRepository.AddAsync(user).Returns(Result<Guid>.Failure("User name must be unique."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await userRepository.Received(1).AddAsync(user);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User name must be unique.");
        }

        [Fact]
        public async Task Given_ExistingEmailForCreateUserCommand_When_HandlerISCalled_Then_UserShouldNotBeCreated()
        {
            var command = new CreateUserCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garci@gamil.com",
                Password = "Password123!",
            };

            var user = new User
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                PasswordHash = passwordHasher.HashPassword(command.Password),
                Role = "User",
                LoginProvider = "Local",
            };
            mapper.Map<User>(command).Returns(user);
            userRepository.AddAsync(user).Returns(Result<Guid>.Failure("Email must be unique."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await userRepository.Received(1).AddAsync(user);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Email must be unique.");
        }

        [Fact]
        public async Task Given_EmptyEmailForCreateUserCommand_When_HandlerISCalled_Then_UserShouldNotBeCreated()
        {
            var command = new CreateUserCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "",
                Password = "Password123!",
            };

            var user = new User
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                PasswordHash = passwordHasher.HashPassword(command.Password),
                Role = "User",
                LoginProvider = "Local",
            };
            mapper.Map<User>(command).Returns(user);
            userRepository.AddAsync(user).Returns(Result<Guid>.Failure("Email is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await userRepository.Received(1).AddAsync(user);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Email is required.");
        }

        [Fact]
        public async Task Given_InvalidFormatEmailForCreateUserCommand_When_HandlerISCalled_Then_UserShouldNotBeCreated()
        {
            var command = new CreateUserCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garci.gamil.com",
                Password = "Password123!",
            };

            var user = new User
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                PasswordHash = passwordHasher.HashPassword(command.Password),
                Role = "User",
                LoginProvider = "Local",
            };
            mapper.Map<User>(command).Returns(user);
            userRepository.AddAsync(user).Returns(Result<Guid>.Failure("Invalid email format."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await userRepository.Received(1).AddAsync(user);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Invalid email format.");
        }

        [Fact]
        public async Task Given_EmptyPasswordForCreateUserCommand_When_HandlerISCalled_Then_UserShouldNotBeCreated()
        {
            var command = new CreateUserCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                Password = "",
            };

            var user = new User
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                PasswordHash = passwordHasher.HashPassword(command.Password),
                Role = "User",
                LoginProvider = "Local",
            };
            mapper.Map<User>(command).Returns(user);
            userRepository.AddAsync(user).Returns(Result<Guid>.Failure("Password is required for local login."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await userRepository.Received(1).AddAsync(user);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password is required for local login.");
        }

        [Fact]
        public async Task Given_TooShortPasswordForCreateUserCommand_When_HandlerISCalled_Then_UserShouldNotBeCreated()
        {
            var command = new CreateUserCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                Password = "Ab1",
            };

            var user = new User
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                PasswordHash = passwordHasher.HashPassword(command.Password),
                Role = "User",
                LoginProvider = "Local",
            };
            mapper.Map<User>(command).Returns(user);
            userRepository.AddAsync(user).Returns(Result<Guid>.Failure("Password must be between 6 and 50 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await userRepository.Received(1).AddAsync(user);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password must be between 6 and 50 characters.");
        }

        [Fact]
        public async Task Given_PasswordWithoutSpecialCharacterForCreateUserCommand_When_HandlerISCalled_Then_UserShouldNotBeCreated()
        {
            var command = new CreateUserCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                Password = "Password123", // Fără caracter special
                LoginProvider = "local"
            };

            var user = new User
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                PasswordHash = passwordHasher.HashPassword(command.Password),
                Role = "User",
                LoginProvider = "local"
            };

            mapper.Map<User>(command).Returns(user);
            userRepository.AddAsync(user).Returns(Result<Guid>.Failure("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await userRepository.Received(1).AddAsync(user);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
        }

        [Fact]
        public async Task Given_PasswordWithoutUppercaseLetterForCreateUserCommand_When_HandlerISCalled_Then_UserShouldNotBeCreated()
        {
            var command = new CreateUserCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                Password = "password123!",
                LoginProvider = "local"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                PasswordHash = passwordHasher.HashPassword(command.Password),
                Role = "User",
                LoginProvider = "local"
            };

            mapper.Map<User>(command).Returns(user);
            userRepository.AddAsync(user).Returns(Result<Guid>.Failure("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character."));

            var result = await handler.Handle(command, CancellationToken.None);

            await userRepository.Received(1).AddAsync(user);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
        }

        [Fact]
        public async Task Given_PasswordWithoutLowercaseLetterForCreateUserCommand_When_HandlerISCalled_Then_UserShouldNotBeCreated()
        {
            var command = new CreateUserCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                Password = "PASSWORD123!",
                LoginProvider = "local"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                PasswordHash = passwordHasher.HashPassword(command.Password),
                Role = "User",
                LoginProvider = "local"
            };

            mapper.Map<User>(command).Returns(user);
            userRepository.AddAsync(user).Returns(Result<Guid>.Failure("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character."));

            var result = await handler.Handle(command, CancellationToken.None);

            await userRepository.Received(1).AddAsync(user);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
        }

        [Fact]
        public async Task Given_PasswordWithoutNumberForCreateUserCommand_When_HandlerISCalled_Then_UserShouldNotBeCreated()
        {
            var command = new CreateUserCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                Password = "Password!",
                LoginProvider = "local"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                PasswordHash = passwordHasher.HashPassword(command.Password),
                Role = "User",
                LoginProvider = "local"
            };

            mapper.Map<User>(command).Returns(user);
            userRepository.AddAsync(user).Returns(Result<Guid>.Failure("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character."));

            var result = await handler.Handle(command, CancellationToken.None);

            await userRepository.Received(1).AddAsync(user);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
        }

    }
}
