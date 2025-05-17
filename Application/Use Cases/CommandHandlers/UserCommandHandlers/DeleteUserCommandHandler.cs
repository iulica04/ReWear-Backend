using Application.Use_Cases.Commands.UserCommands;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.UserCommandHandlers
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<Unit>>
    {
        private readonly IUserRepository repository;

        public DeleteUserCommandHandler(IUserRepository repository)
        {
            this.repository = repository;
        }
        public async Task<Result<Unit>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await repository.GetByIdAsync(request.Id);
            if (user == null)
            {
                return Result<Unit>.Failure("User not found");
            }
            await repository.DeleteAsync(user.Id);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
