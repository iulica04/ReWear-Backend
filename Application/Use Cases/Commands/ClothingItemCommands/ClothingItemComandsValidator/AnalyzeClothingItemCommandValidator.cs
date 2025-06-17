using Application.Use_Cases.Commands.ClothingItemCommand;
using FluentValidation;

namespace Application.Use_Cases.Commands.ClothingItemCommands.ClothingItemComandsValidator
{
    public class AnalyzeClothingItemCommandValidator : AbstractValidator<AnalyzeClothingItemCommand>
    {
        public AnalyzeClothingItemCommandValidator()
        {
            // Validare pentru ImageFront
            RuleFor(c => c.ImageFront)
                .NotEmpty()
                .WithMessage("Front image is required.")
                .Must(image => image != null && image.Length > 0)
                .WithMessage("Front image data cannot be empty.");

        }
    }
}