using Domain.Common;

namespace Application.Services
{
    public interface IEmailService
    {
        Task<Result<string>> SendResetPasswordCodeEmailAsync(string toEmail, string code);
        Task<Result<string>> SendEmail(string toEmail, string subject, string body);
    }
}
