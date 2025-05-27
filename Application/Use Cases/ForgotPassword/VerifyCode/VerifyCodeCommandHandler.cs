using Application.Services;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.ForgotPassword.VerifyCode
{
    public class VerifyCodeCommandHandler : IRequestHandler<VerifyCodeCommand, Result<string>>
    {
        private readonly IPasswordResetCodeRepository passwordResetCodeRepository;
        private readonly IUserRepository userRepository;
        private readonly IEmailService emailService;
        public VerifyCodeCommandHandler(IPasswordResetCodeRepository passwordResetCodeRepository, IEmailService emailService, IUserRepository userRepository)
        {
            this.passwordResetCodeRepository = passwordResetCodeRepository;
            this.emailService = emailService;
            this.userRepository = userRepository;
        }
        public async Task<Result<string>> Handle(VerifyCodeCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return Result<string>.Failure("Email not found");
            }

            var code = await passwordResetCodeRepository.GetByCodeAsync(request.Code);
            if (code == null)
            {
                return Result<string>.Failure("Invalid code");
            }
            if (code.UserId != user.Id)
            {
                return Result<string>.Failure("Code does not match the email");
            }

            if (code.IsUsed)
            {
                return Result<string>.Failure("Code has already been used");
            }

            if (code.Attempts >= 3)
            {
                return Result<string>.Failure("Code has expired");
            }

            // Mark the code as used
            code.IsUsed = true;
            code.Attempts++;

            await passwordResetCodeRepository.UpdateAsync(code);
            return Result<string>.Success("Code verified successfully");
        }
    }

}
