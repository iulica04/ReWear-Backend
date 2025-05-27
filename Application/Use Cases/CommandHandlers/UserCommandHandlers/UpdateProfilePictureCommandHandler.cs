using Application.Services;
using Application.Use_Cases.Commands.UserCommands;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.UserCommandHandlers
{
    public class UpdateProfilePictureCommandHandler : IRequestHandler<UpdateProfilePictureCommand, Result<string>>
    {
        private readonly IUserRepository repository;
        private readonly IImageManagementService imageManagementService;
        public UpdateProfilePictureCommandHandler(IUserRepository repository, IImageManagementService imageManagementService)
        {
            this.repository = repository;
            this.imageManagementService = imageManagementService;
        }
        public async Task<Result<string>> Handle(UpdateProfilePictureCommand request, CancellationToken cancellationToken)
        {
            var user = await repository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Result<string>.Failure("User not found");
            }
            // Check if the file is not null and has content
            if (request.ProfilePicture == null || request.ProfilePicture.Length == 0)
            {
                return Result<string>.Failure("No file uploaded");
            }

            // Upload the image and get the URL
            var uploadResult = await imageManagementService.UploadProfilePictureAsync(request.ProfilePicture, request.UserId.ToString());
            if (!uploadResult.IsSuccess)
            {
                return Result<string>.Failure("Failed to upload image");
            }
            // Update the user's profile picture URL
            user.ProfilePicture = uploadResult.Data;
            await repository.UpdateAsync(user);
            return Result<string>.Success(uploadResult.Data);
        }
    }
}
