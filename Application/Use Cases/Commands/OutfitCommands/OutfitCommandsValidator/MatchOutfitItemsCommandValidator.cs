using Application.Use_Cases.Commands.OutfitCommands;
using Domain.Repositories;
using FluentValidation;
using System;

namespace Application.Use_Cases.Commands.OutfitCommands.OutfitCommandsValidator
{
    public class MatchOutfitItemsCommandValidator : AbstractValidator<MatchOutfitItemsCommand>
    {
        public MatchOutfitItemsCommandValidator(IUserRepository userRepository)
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

            // Name (optional, but if present, max 50 chars)
            When(x => !string.IsNullOrEmpty(x.Name), () =>
            {
                RuleFor(x => x.Name)
                    .MaximumLength(50)
                    .WithMessage("Name cannot exceed 50 characters.");
            });

            // Category (optional, but if present, max 30 chars)
            When(x => !string.IsNullOrEmpty(x.Category), () =>
            {
                RuleFor(x => x.Category)
                    .MaximumLength(30)
                    .WithMessage("Category cannot exceed 30 characters.");
            });

            // Tags (optional, but if present, max 10 tags, each max 30 chars)
            When(x => x.Tags != null && x.Tags.Count > 0, () =>
            {
                RuleFor(x => x.Tags)
                    .Must(tags => tags.Count <= 10)
                    .WithMessage("You can specify at most 10 tags.");

                RuleForEach(x => x.Tags)
                    .MaximumLength(30)
                    .WithMessage("Each tag cannot exceed 30 characters.");
            });

            // Color (optional, max 20 chars)
            When(x => !string.IsNullOrEmpty(x.Color), () =>
            {
                RuleFor(x => x.Color)
                    .MaximumLength(20)
                    .WithMessage("Color cannot exceed 20 characters.");
            });

            // Brand (optional, max 30 chars)
            When(x => !string.IsNullOrEmpty(x.Brand), () =>
            {
                RuleFor(x => x.Brand)
                    .MaximumLength(30)
                    .WithMessage("Brand cannot exceed 30 characters.");
            });

            // Material (optional, max 30 chars)
            When(x => !string.IsNullOrEmpty(x.Material), () =>
            {
                RuleFor(x => x.Material)
                    .MaximumLength(30)
                    .WithMessage("Material cannot exceed 30 characters.");
            });

            // PrintType (optional, max 30 chars)
            When(x => !string.IsNullOrEmpty(x.PrintType), () =>
            {
                RuleFor(x => x.PrintType)
                    .MaximumLength(30)
                    .WithMessage("Print type cannot exceed 30 characters.");
            });

            // PrintDescription (optional, max 100 chars)
            When(x => !string.IsNullOrEmpty(x.PrintDescription), () =>
            {
                RuleFor(x => x.PrintDescription)
                    .MaximumLength(100)
                    .WithMessage("Print description cannot exceed 100 characters.");
            });

            // Description (optional, max 500 chars)
            When(x => !string.IsNullOrEmpty(x.Description), () =>
            {
                RuleFor(x => x.Description)
                    .MaximumLength(500)
                    .WithMessage("Description cannot exceed 500 characters.");
            });
        }
    }
}