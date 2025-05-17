using Domain.Repositories;
using FluentValidation;

namespace Application.Use_Cases.Commands.UserCommands.UserComandsValidator
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator(IUserRepository userRepository) 
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required.")
                .Length(2, 30)
                .WithMessage("First name must be between 2 and 30 characters.");
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required.")
                .Length(2, 30)
                .WithMessage("Last name must be between 2 and 30 characters.");

            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("User name is required.")
                .Length(2, 30)
                .WithMessage("User name must be between 2 and 30 characters.")
                .MustAsync(async (userName, cancellation) =>
                {
                    return !await userRepository.UserNameExistsAsync(userName);
                })
                .WithMessage("User name must be unique.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.")
                .MustAsync(async (email, cancellation) =>
                {
                    return !await userRepository.EmailExistsAsync(email);
                })
                .WithMessage("Email must be unique.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .Length(6, 50)
                .WithMessage("Password must be between 6 and 50 characters.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{6,}$")
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one number.");

        }
    }
}
