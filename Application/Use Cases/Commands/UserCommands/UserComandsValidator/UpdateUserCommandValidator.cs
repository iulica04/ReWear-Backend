using Domain.Repositories;
using FluentValidation;

namespace Application.Use_Cases.Commands.UserCommands.UserComandsValidator
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator(IUserRepository userRepository)
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
            .MustAsync(async (command, userName, cancellation) =>
            {
                var existingUser = await userRepository.GetByUserNameAsync(userName);
                return existingUser == null || existingUser.Id == command.Id;
            })
            .WithMessage("User name must be unique.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.")
                .MustAsync(async (command, email, cancellation) =>
                {
                    var existingUser = await userRepository.GetByEmailAsync(email);
                    return existingUser == null || existingUser.Id == command.Id;
                })
                .WithMessage("Email must be unique.");

            RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .When(x => x.LoginProvider == "Local")
            .WithMessage("Password is required for local login.")
            .Length(6, 50)
            .When(x => x.LoginProvider == "Local")
            .WithMessage("Password must be between 6 and 50 characters.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{6,}$")
            .When(x => x.LoginProvider == "Local")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one number.");



            RuleFor(x => x.Id)
                .NotEmpty()
                .Must(BeAValidGuid);

        }

        private bool BeAValidGuid(Guid id)
        {
            return Guid.TryParse(id.ToString(), out _);
        }
    }
}