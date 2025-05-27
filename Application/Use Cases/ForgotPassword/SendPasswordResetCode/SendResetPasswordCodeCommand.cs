using Domain.Common;
using MediatR;

namespace Application.Use_Cases.ForgotPassword.SendPasswordResetCode
{
    public class SendResetPasswordCodeCommand : IRequest<Result<string>>
    {
        public required string Email { get; set; }
    }
}
