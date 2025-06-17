using Application.Services;
using Application.Use_Cases.Commands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<string>>
    {
        private readonly IUserRepository repository;
        private readonly IMapper mapper;
        private readonly IPasswordHasher passwordHasher;

        public UpdateUserCommandHandler(IUserRepository repository, IMapper mapper, IPasswordHasher passwordHasher)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.passwordHasher = passwordHasher;
        }

        public async Task<Result<string>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await repository.GetByIdAsync(request.Id);
            if (user == null)
            {
                return Result<string>.Failure("User not found");
            }

            Console.WriteLine("passwordHash" + user.PasswordHash);
            Console.WriteLine(request.Password);
            Console.WriteLine(passwordHasher.HashPassword(request.Password!));

            if (!passwordHasher.Verify(user.PasswordHash!, request.Password!))
            {
                Console.WriteLine("Incorect password");
                return Result<string>.Failure("Incorect password");
            }

            // Dacă userul vrea să treacă de pe Google pe Local
            if (user.LoginProvider.Equals("Google", StringComparison.OrdinalIgnoreCase) &&
                request.LoginProvider.Equals("Local", StringComparison.OrdinalIgnoreCase))
            {
                user.LoginProvider = "Local";
                user.PasswordHash = passwordHasher.HashPassword(request.Password!);
                user.GoogleId = null;
            }

            // Updatează restul datelor (poți folosi mapper sau manual)
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.UserName = request.UserName;
            user.Email = request.Email;
            if (!string.IsNullOrEmpty(request.Role))
                user.Role = request.Role;

            var result = await repository.UpdateAsync(user);
            if (result.IsSuccess)
            {
                return Result<string>.Success("User updated successfully");
            }
            else
            {
                return Result<string>.Failure(result.ErrorMessage);
            }

        }
    }
}