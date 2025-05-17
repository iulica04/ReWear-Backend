using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.UserCommands
{
    public record DeleteUserCommand(Guid Id) : IRequest<Result<Unit>>
    {
    }
}
