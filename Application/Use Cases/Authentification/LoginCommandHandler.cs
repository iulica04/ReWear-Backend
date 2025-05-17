using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<string>>
    {
        private readonly IUserRepository repository;

        public LoginCommandHandler(IUserRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<string>> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            // Call the repository to perform login
            var token = await repository.Login(command.Email, command.Password);

            // Check if the token is null or empty
            if (string.IsNullOrEmpty(token))
            {
                return Result<string>.Failure("Invalid credentials");
            }

            // Return success with the token
            return Result<string>.Success(token);
        }
    }
}