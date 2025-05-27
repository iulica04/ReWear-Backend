using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResult>>
    {
        private readonly IUserRepository repository;

        public LoginCommandHandler(IUserRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<LoginResult>> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var user = await repository.GetByEmailAsync(command.Email);
            if (user == null)
            {
                return Result<LoginResult>.Failure("No account found associated with this email address. Please check if the email is correct or consider creating a new account.");
            }
            // Call the repository to perform login
            var loginResult = await repository.Login(command.Email, command.Password);

            // Check if the token is null or empty
            if (loginResult == null || string.IsNullOrEmpty(loginResult.Token))
            {
                return Result<LoginResult>.Failure("Login failed: Invalid credentials. Retry or reset password.");
            }

            // Return success with the token
            return Result<LoginResult>.Success(loginResult);
        }
    }
}