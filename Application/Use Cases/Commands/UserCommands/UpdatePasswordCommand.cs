using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.UserCommands
{
    public class UpdatePasswordCommand : IRequest<Result<string>>
    {
        public required Guid UserId { get; set; }
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
