using Application.Use_Cases.Commands.OutfitCommands;
using Domain.Repositories;
using FluentValidation;

namespace Application.Use_Cases.Commands.OutfitCommands.OutfitCommandsValidator
{
    public class UpdateOutfitCommandValidator : AbstractValidator<UpdateOutfitCommand>
    {
        public UpdateOutfitCommandValidator(
            IUserRepository userRepository,
            IOutfitRepository outfitRepository,
            IClothingItemRepository clothingItemRepository)
        {
            // Validate Outfit Id
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Outfit ID is required.")
                .MustAsync(async (id, cancellation) =>
                {
                    var outfit = await outfitRepository.GetByIdAsync(id);
                    return outfit != null;
                })
                .WithMessage("The specified outfit does not exist.");

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

            // Validate ClothingItemIds (optional, but if present, must exist)
            When(x => x.ClothingItemIds != null && x.ClothingItemIds.Count > 0, () =>
            {
                RuleForEach(x => x.ClothingItemIds)
                    .MustAsync(async (itemId, cancellation) =>
                    {
                        var item = await clothingItemRepository.GetByIdAsync(itemId);
                        return item != null;
                    })
                    .WithMessage("One or more clothing items do not exist.");
            });

            // Season (optional, max 20 chars)
            When(x => !string.IsNullOrEmpty(x.Season), () =>
            {
                RuleFor(x => x.Season)
                    .MaximumLength(20)
                    .WithMessage("Season cannot exceed 20 characters.");
            });

            // Description (optional, max 500 chars)
            When(x => !string.IsNullOrEmpty(x.Description), () =>
            {
                RuleFor(x => x.Description)
                    .MaximumLength(500)
                    .WithMessage("Description cannot exceed 500 characters.");
            });

            // ImageFront (optional, but if present, must not be empty)
            When(x => x.ImageFront != null, () =>
            {
                RuleFor(x => x.ImageFront)
                    .Must(img => img.Length > 0)
                    .WithMessage("ImageFront cannot be empty if provided.");
            });
        }
    }
}