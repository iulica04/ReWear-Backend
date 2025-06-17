
using Application.Use_Cases.Commands.OutfitCommands;
using Domain.Repositories;
using FluentValidation;

namespace Application.Use_Cases.Commands.OutfitCommands.OutfitCommandsValidator
{
    public class CreateOutfitCommandValidator : AbstractValidator<CreateOutfitCommand>
    {
        public CreateOutfitCommandValidator(
            IUserRepository userRepository,
            IClothingItemRepository clothingItemRepository)
        {
            // Validate UserId
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.")
                .MustAsync(async (userId, cancellation) =>
                {
                    var user = await userRepository.GetByIdAsync(userId);
                    return user != null;
                })
                .WithMessage("The specified user does not exist.");

            // Validate Name
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Outfit name is required.")
                .MaximumLength(50)
                .WithMessage("Outfit name cannot exceed 50 characters.");

            // Validate Style
            RuleFor(x => x.Style)
                .NotEmpty()
                .WithMessage("Style is required.")
                .MaximumLength(30)
                .WithMessage("Style cannot exceed 30 characters.");

            // Validate ClothingItemIds - ensure they exist and belong to the user
            RuleFor(x => x.ClothingItemIds)
                .NotEmpty()
                .WithMessage("At least one clothing item is required for an outfit.");
                

            // Validate Season (optional)
            When(x => !string.IsNullOrEmpty(x.Season), () =>
            {
                RuleFor(x => x.Season)
                    .MaximumLength(20)
                    .WithMessage("Season cannot exceed 20 characters.");
            });

            // Validate Description (optional)
            When(x => !string.IsNullOrEmpty(x.Description), () =>
            {
                RuleFor(x => x.Description)
                    .MaximumLength(500)
                    .WithMessage("Description cannot exceed 500 characters.");
            });

            // Validate ImageFront
            RuleFor(x => x.ImageFront)
                .NotEmpty()
                .WithMessage("Image is required.")
                .Must(image => image != null && image.Length > 0)
                .WithMessage("Image data cannot be empty.");
          
        }
    }
}