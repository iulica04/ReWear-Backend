using Application.Use_Cases.Commands.FavoriteOutfitCommands;
using Domain.Repositories;
using FluentValidation;

namespace Application.Use_Cases.Commands.FavoriteOutfitCommands.FavoriteOutfitCommandsValidator
{
    public class CreateFavoriteOutfitCommandValidator : AbstractValidator<CreateFavoriteOutfitCommand>
    {
        public CreateFavoriteOutfitCommandValidator(IUserRepository userRepository, IOutfitRepository outfitRepository, IFavoriteOutfitRepository favoriteOutfitRepository)
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

            // Validate OutfitId
            RuleFor(x => x.OutfitId)
                .NotEmpty()
                .WithMessage("Outfit ID is required.")
                .MustAsync(async (outfitId, cancellation) =>
                {
                    var outfit = await outfitRepository.GetByIdAsync(outfitId);
                    return outfit != null;
                })
                .WithMessage("The specified outfit does not exist.");

            // Validate that the combination of UserId and OutfitId is unique
            RuleFor(x => x)
                .MustAsync(async (command, cancellation) =>
                {
                    var existingFavorite = await favoriteOutfitRepository.GetByUserAndOutfitAsync(command.UserId, command.OutfitId);
                    return existingFavorite == null;
                })
                .WithMessage("This outfit is already in the user's favorites.");
        }
    }
}