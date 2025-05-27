using Application.Services;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.ForgotPassword.SendPasswordResetCode
{
    public class SendResetPasswordCodeCommandHandler : IRequestHandler<SendResetPasswordCodeCommand, Result<string>>
    {
        private readonly IEmailService emailService;
        private readonly IPasswordResetCodeRepository passwordResetCodeRepository;
        private readonly IUserRepository userRepository;

        public SendResetPasswordCodeCommandHandler(IEmailService emailService, IPasswordResetCodeRepository passwordResetCodeRepository, IUserRepository userRepository)
        {
            this.emailService = emailService;
            this.passwordResetCodeRepository = passwordResetCodeRepository;
            this.userRepository = userRepository;
        }
        public async Task<Result<string>> Handle(SendResetPasswordCodeCommand request, CancellationToken cancellationToken)
        {

            // Check if the email exists in the database
            var user = await userRepository.GetByEmailAsync(request.Email);
            if (user == null) {
                return Result<string>.Failure("Email not found");
            }

            var code = GenerateRandomCode();
            while(passwordResetCodeRepository.ExistsAsync(code).Result)
            {
                code = GenerateRandomCode();
            }
            var result = await emailService.SendResetPasswordCodeEmailAsync(request.Email, code);

            if (result.IsSuccess)
            {
                // Save the code to the database
                var savedCode = new PasswordResetCode
                {
                    Code = code,
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    Attempts = 0,
                    IsUsed = false,
                };
                var saveResult = await passwordResetCodeRepository.AddAsync(savedCode);
                if (!saveResult.IsSuccess)
                {
                    return Result<string>.Failure("Failed to save the code to the database");
                }
                // Return success
                return Result<string>.Success("Email sent successfully");
            }
            return Result<string>.Failure(result.ErrorMessage);
        }
        private string GenerateRandomCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
