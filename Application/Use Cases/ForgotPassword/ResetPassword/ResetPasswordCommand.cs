using Domain.Common;
using MediatR;

namespace Application.Use_Cases.ForgotPassword.ResetPassword
{
    public class ResetPasswordCommand : IRequest<Result<string>>
    {
        public required string Email { get; set; }
        public required string NewPassword { get; set; }
    }
}
