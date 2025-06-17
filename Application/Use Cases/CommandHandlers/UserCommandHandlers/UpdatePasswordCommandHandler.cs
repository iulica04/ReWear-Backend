using Application.Services;
using Application.Use_Cases.Commands.UserCommands;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.UserCommandHandlers
{
    public class UpdatePasswordCommandHandle : IRequestHandler<UpdatePasswordCommand, Result<string>>
    {
        private readonly IUserRepository userRepository;
        private readonly IPasswordHasher passwordHasher;

        public UpdatePasswordCommandHandle(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
        }
        public async Task<Result<string>> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Result<string>.Failure("User not found");
            }

            if(!passwordHasher.Verify(user.PasswordHash, request.OldPassword))
            {
                return Result<string>.Failure("Old password is incorrect");
            }

            user.PasswordHash = passwordHasher.HashPassword(request.NewPassword);
            var result = await userRepository.UpdateAsync(user);
            if (result.IsSuccess)
            {
                return Result<string>.Success("Password updated successfully");
            }
            else
            {
                return Result<string>.Failure(result.ErrorMessage);
            }
        }
    }
}
