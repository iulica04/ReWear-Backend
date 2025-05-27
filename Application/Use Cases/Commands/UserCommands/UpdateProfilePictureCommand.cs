using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.UserCommands
{
    public class UpdateProfilePictureCommand : IRequest<Result<string>>
    {
        public required Guid UserId { get; set; }
        public required byte[] ProfilePicture { get; set; }
    }
}
