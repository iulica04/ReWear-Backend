using Application.Services;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.ForgotPassword.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<string>>
    {
        private readonly IPasswordResetCodeRepository passwordResetCodeRepository;
        private readonly IUserRepository userRepository;
        private readonly IEmailService emailService;
        private readonly IPasswordHasher passwordHasher;
        public ResetPasswordCommandHandler(IPasswordResetCodeRepository passwordResetCodeRepository, IEmailService emailService, IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            this.passwordResetCodeRepository = passwordResetCodeRepository;
            this.emailService = emailService;
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
        }
        public async Task<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return Result<string>.Failure("Email not found");
            }
            user.PasswordHash = passwordHasher.HashPassword(request.NewPassword);
            await userRepository.UpdateAsync(user);
            var subject = "Password Reset Confirmation";
            var body = "<!DOCTYPE html>\n<html lang=\"en\">\n<head>\n  <meta charset=\"UTF-8\" />\n  <title>Password Reset</title>\n  <style>\n    body {\n      font-family: Arial, sans-serif;\n      color: #51423e;\n      text-align: center;\n      padding: 50px;\n    }\n    .container {\n      background-color: #efebe2;\n      padding: 25px 20px;\n      border-radius: 12px;\n      display: inline-block;\n      width: 90%;\n      max-width: 360px;\n    }\n    .logo {\n      width: 100%;\n      max-width: 350px;\n      border-radius: 10px 10px 0 0;\n      margin-bottom: 20px;\n    }\n\n    p {\n      margin-top: 20px;\n      font-size: 16px;\n    }\n  </style>\n</head>\n<body>\n  <div class=\"container\">\n    <img src=\"https://storage.googleapis.com/rewear/logo3.png\" alt=\"Rewear Logo\" class=\"logo\" />\n    <h2>Password Changed Successfully</h2>\n    <p>Your password for your Rewear account has been changed successfully.</p>\n    <p>If you did not perform this change, please contact our support immediately.</p>\n    <p>Thank you for using Rewear!</p>\n  </div>\n</body>\n</html>";
            var result = await emailService.SendEmail(request.Email, subject, body);
            if (result.IsSuccess)
            {
                return Result<string>.Success("Password reset successfully");
            }
            return Result<string>.Failure(result.ErrorMessage);
        }
    }
}
