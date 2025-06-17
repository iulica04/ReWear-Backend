using Application.Use_Cases.Commands.ClothingItemCommand;
using Domain.Repositories;
using FluentValidation;

namespace Application.Use_Cases.Commands.ClothingItemCommands.ClothingItemComandsValidator
{
    public class CreateClothingItemCommandValidator : AbstractValidator<CreateClothingItemCommand>
    {
        public CreateClothingItemCommandValidator(IClothingItemRepository clothingItemRepository)
        {
            // Validare ID utilizator
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");

            // Validare nume articol
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(50)
                .WithMessage("Name cannot exceed 50 characters.");

            // Validare categorie
            RuleFor(x => x.Category)
                .NotEmpty()
                .WithMessage("Category is required.")
                .MaximumLength(50)
                .WithMessage("Category cannot exceed 50 characters.");

            // Validare Tags - dacă există, fiecare tag nu poate depăși 30 caractere
            RuleForEach(x => x.Tags)
                .MaximumLength(30)
                .WithMessage("Each tag cannot exceed 30 characters.");

            // Validare culoare
            RuleFor(x => x.Color)
                .NotEmpty()
                .WithMessage("Color is required.")
                .MaximumLength(20)
                .WithMessage("Color cannot exceed 20 characters.");

            // Validare brand
            RuleFor(x => x.Brand)
                .NotEmpty()
                .WithMessage("Brand is required.")
                .MaximumLength(30)
                .WithMessage("Brand cannot exceed 30 characters.");

            // Validare material
            RuleFor(x => x.Material)
                .NotEmpty()
                .WithMessage("Material is required.")
                .MaximumLength(30)
                .WithMessage("Material cannot exceed 30 characters.");

            // Validare PrintType - opțional, dar dacă există, nu poate depăși 30 caractere
            When(x => !string.IsNullOrEmpty(x.PrintType), () =>
            {
                RuleFor(x => x.PrintType)
                    .MaximumLength(30)
                    .WithMessage("Print type cannot exceed 30 characters.");
            });

            // Validare PrintDescription - opțional, dar dacă există, nu poate depăși 100 caractere
            When(x => !string.IsNullOrEmpty(x.PrintDescription), () =>
            {
                RuleFor(x => x.PrintDescription)
                    .MaximumLength(100)
                    .WithMessage("Print description cannot exceed 100 characters.");
            });

            // Validare Description - opțional, dar dacă există, nu poate depăși 2000 caractere
            When(x => !string.IsNullOrEmpty(x.Description), () =>
            {
                RuleFor(x => x.Description)
                    .MaximumLength(2000)
                    .WithMessage("Description cannot exceed 2000 characters.");
            });

            // Validare ImageFront - obligatoriu și trebuie să conțină date
            RuleFor(x => x.ImageFront)
                .NotEmpty()
                .WithMessage("Front image is required.")
                .Must(image => image != null && image.Length > 0)
                .WithMessage("Front image data cannot be empty.");

            // Validare Weight - opțional, dar dacă există, trebuie să fie pozitiv
            When(x => x.Weight.HasValue, () =>
            {
                RuleFor(x => x.Weight!.Value)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Weight must be a positive value.");
            });
        }
    }
}