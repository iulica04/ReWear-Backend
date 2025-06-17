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
    public class UpdateUserCommandHandlerTests
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly UpdateUserCommandHandler handler;
        private readonly IPasswordHasher passwordHasher;
        public UpdateUserCommandHandlerTests()
        {
            this.userRepository = Substitute.For<IUserRepository>();
            this.mapper = Substitute.For<IMapper>();
            this.passwordHasher = Substitute.For<IPasswordHasher>();
            this.handler = new UpdateUserCommandHandler(userRepository, mapper, passwordHasher);
        }

        [Fact]
        public async Task Given_ValidUpdateUserCommand_When_HandlerIsCalled_Then_UserIsUpdatedSuccessfully()
        {
            var command = new UpdateUserCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                Password = "password123!",
                LoginProvider = "Local"
            };

            var user = new User
            {
                Id = command.Id,
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                PasswordHash = "old-password-hash",
                Role = "User",
                LoginProvider = "Google", // SCHIMBAT la "Google"
                GoogleId = "google-id-123" // Adăugat GoogleId
            };

            userRepository.GetByIdAsync(command.Id).Returns(user);
            passwordHasher.HashPassword(command.Password).Returns("hashed-password");
            userRepository.UpdateAsync(user).Returns(Task.FromResult(Result<string>.Success("User updated successfully")));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await userRepository.Received(1).UpdateAsync(Arg.Is<User>(u =>
                u.LoginProvider.Equals("Local", StringComparison.OrdinalIgnoreCase) &&
                u.PasswordHash == "hashed-password" &&
                u.GoogleId == null &&
                u.FirstName == command.FirstName &&
                u.LastName == command.LastName &&
                u.UserName == command.UserName &&
                u.Email == command.Email &&
                u.Role == command.Role
            ));

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be("User updated successfully");
        }

        [Fact]
        public async Task Given_NonExistentUserId_When_HandlerIsCalled_Then_FailureIsReturned()
        {
            var command = new UpdateUserCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Updated",
                LastName = "User",
                UserName = "updated.user",
                Email = "updated@example.com",
                Password = "Password123!",
                LoginProvider = "local"
            };

            userRepository.GetByIdAsync(command.Id).Returns((User?)null);

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User not found");
        }

        [Fact]
        public async Task Given_UserAlreadyUsingLocalProvider_When_HandlerIsCalled_Then_PasswordIsNotUpdated()
        {
            var command = new UpdateUserCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Updated",
                LastName = "User",
                UserName = "updated.user",
                Email = "updated@example.com",
                Password = "Password123!",
                LoginProvider = "local"
            };

            var existingUser = new User
            {
                Id = command.Id,
                FirstName = "Old",
                LastName = "User",
                UserName = "old.user",
                Email = "old@example.com",
                LoginProvider = "local",
                PasswordHash = "old-password-hash"
            };

            userRepository.GetByIdAsync(command.Id).Returns(existingUser);
            userRepository.UpdateAsync(existingUser).Returns(Task.FromResult(Result<string>.Success("User updated successfully")));

            var result = await handler.Handle(command, CancellationToken.None);

            existingUser.PasswordHash.Should().Be("old-password-hash");
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Given_FirstNameGreaterThan30Characters_When_UpdateHandlerIsCalled_Then_UserShouldNotBeUpdated()
        {
            var command = new UpdateUserCommand
            {
                Id = Guid.NewGuid(),
                FirstName = "ThisIsANameThatIsWayTooLongToBeValidInTheSystem",
                LastName = "Doe",
                UserName = "johndoe",
                Email = "john@example.com",
                LoginProvider = "Local",
                Password = "StrongPassword123!"
            };

            var user = new User
            {
                Id = command.Id,
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Email = "john@example.com",
                Role = "User",
                LoginProvider = "Google"
            };

            userRepository.GetByIdAsync(command.Id).Returns(user);
            userRepository.UpdateAsync(Arg.Any<User>())
                .Returns(Result<string>.Failure("First name must be between 2 and 30 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await userRepository.Received(1).UpdateAsync(Arg.Any<User>());
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("First name must be between 2 and 30 characters.");
        }


        [Fact]
        public async Task Given_FirstNameGreaterThan30CharactersForUpdateUserCommand_When_HandlerIsCalled_Then_UserShouldNotBeUpdated()
        {
            var command = new UpdateUserCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "ChristopherAlexanderJohnsonWilliams",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                Password = "password123!",
                LoginProvider = "Local"
            };

            var user = new User
            {
                Id = command.Id,
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                PasswordHash = "old-password-hash",
                Role = "User",
                LoginProvider = "Google"
            };

            userRepository.GetByIdAsync(command.Id).Returns(user);
            userRepository.UpdateAsync(user).Returns(Task.FromResult(Result<string>.Failure("First name must be between 2 and 30 characters.")));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("First name must be between 2 and 30 characters.");
        }

        [Fact]
        public async Task Given_FirstNameLowerThan2CharactersForUpdateUserCommand_When_HandlerIsCalled_Then_UserShouldNotBeUpdated()
        {
            var command = new UpdateUserCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "C",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                Password = "password123!",
                LoginProvider = "Local"
            };

            var user = new User
            {
                Id = command.Id,
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                PasswordHash = "old-password-hash",
                Role = "User",
                LoginProvider = "Google"
            };

            userRepository.GetByIdAsync(command.Id).Returns(user);
            userRepository.UpdateAsync(user).Returns(Task.FromResult(Result<string>.Failure("First name must be between 2 and 30 characters.")));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("First name must be between 2 and 30 characters.");
        }

        [Fact]
        public async Task Given_EmptyLastNameForUpdateUserCommand_When_HandlerIsCalled_Then_UserShouldNotBeUpdated()
        {
            var command = new UpdateUserCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Ethan",
                LastName = "",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                Password = "password123!",
                LoginProvider = "Local"
            };

            var user = new User
            {
                Id = command.Id,
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                PasswordHash = "old-password-hash",
                Role = "User",
                LoginProvider = "Google"
            };

            userRepository.GetByIdAsync(command.Id).Returns(user);
            userRepository.UpdateAsync(user).Returns(Task.FromResult(Result<string>.Failure("Last name is required.")));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Last name is required.");
        }

        [Fact]
        public async Task Given_LastNameGreaterThan30CharactersForUpdateUserCommand_When_HandlerIsCalled_Then_UserShouldNotBeUpdated()
        {
            var command = new UpdateUserCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Ethan",
                LastName = "ChristopherAlexanderJohnsonWilliams",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                Password = "password123!",
                LoginProvider = "Local"
            };

            var user = new User
            {
                Id = command.Id,
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                PasswordHash = "old-password-hash",
                Role = "User",
                LoginProvider = "Google"
            };

            userRepository.GetByIdAsync(command.Id).Returns(user);
            userRepository.UpdateAsync(user).Returns(Task.FromResult(Result<string>.Failure("Last name must be between 2 and 30 characters.")));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Last name must be between 2 and 30 characters.");
        }

        [Fact]
        public async Task Given_LastNameLowerThan2CharactersForUpdateUserCommand_When_HandlerIsCalled_Then_UserShouldNotBeUpdated()
        {
            var command = new UpdateUserCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Ethan",
                LastName = "G",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                Password = "password123!",
                LoginProvider = "Local"
            };

            var user = new User
            {
                Id = command.Id,
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                PasswordHash = "old-password-hash",
                Role = "User",
                LoginProvider = "Google"
            };

            userRepository.GetByIdAsync(command.Id).Returns(user);
            userRepository.UpdateAsync(user).Returns(Task.FromResult(Result<string>.Failure("Last name must be between 2 and 30 characters.")));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Last name must be between 2 and 30 characters.");
        }

        [Fact]
        public async Task Given_EmptyUserNameForUpdateUserCommand_When_HandlerIsCalled_Then_UserShouldNotBeUpdated()
        {
            var command = new UpdateUserCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "",
                Email = "ethan.garcia@gmail.com",
                Password = "password123!",
                LoginProvider = "Local"
            };

            var user = new User
            {
                Id = command.Id,
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                PasswordHash = "old-password-hash",
                Role = "User",
                LoginProvider = "Google"
            };

            userRepository.GetByIdAsync(command.Id).Returns(user);
            userRepository.UpdateAsync(user).Returns(Task.FromResult(Result<string>.Failure("User name is required.")));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User name is required.");
        }

        [Fact]
        public async Task Given_UserNameGreaterThan30CharactersForUpdateUserCommand_When_HandlerIsCalled_Then_UserShouldNotBeUpdated()
        {
            var command = new UpdateUserCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ChristopherAlexanderJohnsonWilliams.garcia",
                Email = "ethan.garcia@gmail.com",
                Password = "password123!",
                LoginProvider = "Local"
            };

            var user = new User
            {
                Id = command.Id,
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                PasswordHash = "old-password-hash",
                Role = "User",
                LoginProvider = "Google"
            };

            userRepository.GetByIdAsync(command.Id).Returns(user);
            userRepository.UpdateAsync(user).Returns(Task.FromResult(Result<string>.Failure("User name must be between 2 and 30 characters.")));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User name must be between 2 and 30 characters.");
        }

        [Fact]
        public async Task Given_UserNameLowerThan2CharactersForUpdateUserCommand_When_HandlerIsCalled_Then_UserShouldNotBeUpdated()
        {
            var command = new UpdateUserCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "e",
                Email = "ethan.garcia@gmail.com",
                Password = "password123!",
                LoginProvider = "Local"
            };

            var user = new User
            {
                Id = command.Id,
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                PasswordHash = "old-password-hash",
                Role = "User",
                LoginProvider = "Google"
            };

            userRepository.GetByIdAsync(command.Id).Returns(user);
            userRepository.UpdateAsync(user).Returns(Task.FromResult(Result<string>.Failure("User name must be between 2 and 30 characters.")));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User name must be between 2 and 30 characters.");
        }

        [Fact]
        public async Task Given_ExistingUserNameForUpdateUserCommand_When_HandlerIsCalled_Then_UserShouldNotBeUpdated()
        {
            var command = new UpdateUserCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "existing.username",
                Email = "ethan.garcia@gmail.com",
                Password = "password123!",
                LoginProvider = "Local"
            };

            var user = new User
            {
                Id = command.Id,
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                PasswordHash = "old-password-hash",
                Role = "User",
                LoginProvider = "Google"
            };

            userRepository.GetByIdAsync(command.Id).Returns(user);
            userRepository.UpdateAsync(user).Returns(Task.FromResult(Result<string>.Failure("User name must be unique.")));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User name must be unique.");
        }

        [Fact]
        public async Task Given_EmptyEmailForUpdateUserCommand_When_HandlerIsCalled_Then_UserShouldNotBeUpdated()
        {
            var command = new UpdateUserCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "",
                Password = "password123!",
                LoginProvider = "Local"
            };

            var user = new User
            {
                Id = command.Id,
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                PasswordHash = "old-password-hash",
                Role = "User",
                LoginProvider = "Google"
            };

            userRepository.GetByIdAsync(command.Id).Returns(user);
            userRepository.UpdateAsync(user).Returns(Task.FromResult(Result<string>.Failure("Email is required.")));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Email is required.");
        }

        [Fact]
        public async Task Given_InvalidFormatEmailForUpdateUserCommand_When_HandlerIsCalled_Then_UserShouldNotBeUpdated()
        {
            var command = new UpdateUserCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia.gmail.com", // Invalid email format
                Password = "password123!",
                LoginProvider = "Local"
            };

            var user = new User
            {
                Id = command.Id,
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                PasswordHash = "old-password-hash",
                Role = "User",
                LoginProvider = "Google"
            };

            userRepository.GetByIdAsync(command.Id).Returns(user);
            userRepository.UpdateAsync(user).Returns(Task.FromResult(Result<string>.Failure("Invalid email format.")));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Invalid email format.");
        }

        [Fact]
        public async Task Given_ExistingEmailForUpdateUserCommand_When_HandlerIsCalled_Then_UserShouldNotBeUpdated()
        {
            var command = new UpdateUserCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "existing.email@gmail.com", // Email already used by another user
                Password = "password123!",
                LoginProvider = "Local"
            };

            var user = new User
            {
                Id = command.Id,
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                PasswordHash = "old-password-hash",
                Role = "User",
                LoginProvider = "Google"
            };

            userRepository.GetByIdAsync(command.Id).Returns(user);
            userRepository.UpdateAsync(user).Returns(Task.FromResult(Result<string>.Failure("Email must be unique.")));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Email must be unique.");
        }

        [Fact]
        public async Task Given_EmptyPasswordWhenSwitchingToLocalForUpdateUserCommand_When_HandlerIsCalled_Then_UserShouldNotBeUpdated()
        {
            var command = new UpdateUserCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                Password = "", // Empty password
                LoginProvider = "Local" // Switching to Local
            };

            var user = new User
            {
                Id = command.Id,
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                PasswordHash = "old-password-hash",
                Role = "User",
                LoginProvider = "Google" // From Google
            };

            userRepository.GetByIdAsync(command.Id).Returns(user);
            userRepository.UpdateAsync(user).Returns(Task.FromResult(Result<string>.Failure("Password is required for local login.")));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password is required for local login.");
        }

        [Fact]
        public async Task Given_TooShortPasswordWhenSwitchingToLocalForUpdateUserCommand_When_HandlerIsCalled_Then_UserShouldNotBeUpdated()
        {
            var command = new UpdateUserCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                Password = "Ab1!", // Too short
                LoginProvider = "Local" // Switching to Local
            };

            var user = new User
            {
                Id = command.Id,
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan.garcia@gmail.com",
                PasswordHash = "old-password-hash",
                Role = "User",
                LoginProvider = "Google" // From Google
            };

            userRepository.GetByIdAsync(command.Id).Returns(user);
            userRepository.UpdateAsync(user).Returns(Task.FromResult(Result<string>.Failure("Password must be between 6 and 50 characters.")));

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password must be between 6 and 50 characters.");
        }

        [Fact]
        public async Task Given_PasswordMissingUppercase_When_HandlerIsCalled_Then_ShouldReturnError()
        {
            var command = new UpdateUserCommand
            {
                Id = Guid.NewGuid(),
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan@example.com",
                Password = "password1!", // fără literă mare
                LoginProvider = "Local"
            };

            var user = new User
            {
                Id = command.Id,
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan@example.com",
                PasswordHash = "$2a$11$Vp3mxEdei672TlcjmWTdPel.OHNrHyd746E2nytTgg7rx7Q7pXb0C",
                LoginProvider = "Google"
            };

            userRepository.GetByIdAsync(command.Id).Returns(user);
            passwordHasher.HashPassword(command.Password).Returns("$2a$11$Vp3mxEdei672TlcjmWTdPel.OHNrHyd746E2nytTgg7rx7Q7pXb0C");

            userRepository.UpdateAsync(Arg.Any<User>())
                .Returns(Task.FromResult(Result<string>.Failure("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")));


            var result = await handler.Handle(command, CancellationToken.None);

            await userRepository.Received(1).UpdateAsync(Arg.Any<User>());
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
        }

        [Fact]
        public async Task Given_PasswordMissingLowercase_When_HandlerIsCalled_Then_ShouldReturnError()
        {
            var command = new UpdateUserCommand
            {
                Id = Guid.NewGuid(),
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan@example.com",
                Password = "PASSWORD1!", // fără literă mică
                LoginProvider = "Local"
            };

            var user = new User
            {
                Id = command.Id,
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan@example.com",
                PasswordHash = "$2a$11$Vp3mxEdei672TlcjmWTdPel.OHNrHyd746E2nytTgg7rx7Q7pXb0C",
                LoginProvider = "Google"
            };

            userRepository.GetByIdAsync(command.Id).Returns(user);
            passwordHasher.HashPassword(command.Password).Returns("$2a$11$Vp3mxEdei672TlcjmWTdPel.OHNrHyd746E2nytTgg7rx7Q7pXb0C");

            userRepository.UpdateAsync(Arg.Any<User>())
                .Returns(Task.FromResult(Result<string>.Failure("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")));


            var result = await handler.Handle(command, CancellationToken.None);

            await userRepository.Received(1).UpdateAsync(Arg.Any<User>());
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
        }

        [Fact]
        public async Task Given_PasswordMissingNumber_When_HandlerIsCalled_Then_ShouldReturnError()
        {
            var command = new UpdateUserCommand
            {
                Id = Guid.NewGuid(),
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan@example.com",
                Password = "Password!", // fără număr
                LoginProvider = "Local"
            };

            var user = new User
            {
                Id = command.Id,
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan@example.com",
                PasswordHash = "$2a$11$Vp3mxEdei672TlcjmWTdPel.OHNrHyd746E2nytTgg7rx7Q7pXb0C",
                LoginProvider = "Google"
            };

            userRepository.GetByIdAsync(command.Id).Returns(user);
            passwordHasher.HashPassword(command.Password).Returns("$2a$11$Vp3mxEdei672TlcjmWTdPel.OHNrHyd746E2nytTgg7rx7Q7pXb0C");

            userRepository.UpdateAsync(Arg.Any<User>())
                .Returns(Task.FromResult(Result<string>.Failure("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")));


            var result = await handler.Handle(command, CancellationToken.None);

            await userRepository.Received(1).UpdateAsync(Arg.Any<User>());
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
        }

        [Fact]
        public async Task Given_PasswordMissingSpecialCharacter_When_HandlerIsCalled_Then_ShouldReturnError()
        {
            var command = new UpdateUserCommand
            {
                Id = Guid.NewGuid(),
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan@example.com",
                Password = "Password123", // fără caracter special
                LoginProvider = "Local"
            };

            var user = new User
            {
                Id = command.Id,
                FirstName = "Ethan",
                LastName = "Garcia",
                UserName = "ethan.garcia",
                Email = "ethan@example.com",
                PasswordHash = "$2a$11$Vp3mxEdei672TlcjmWTdPel.OHNrHyd746E2nytTgg7rx7Q7pXb0C",
                LoginProvider = "Google"
            };

            userRepository.GetByIdAsync(command.Id).Returns(user);
            passwordHasher.HashPassword(command.Password).Returns("$2a$11$Vp3mxEdei672TlcjmWTdPel.OHNrHyd746E2nytTgg7rx7Q7pXb0C");

            userRepository.UpdateAsync(Arg.Any<User>())
                .Returns(Task.FromResult(Result<string>.Failure("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")));


            var result = await handler.Handle(command, CancellationToken.None);

            await userRepository.Received(1).UpdateAsync(Arg.Any<User>());
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
        }
    }
}