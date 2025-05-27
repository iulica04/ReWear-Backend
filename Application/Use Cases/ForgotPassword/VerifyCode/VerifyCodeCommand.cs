using Domain.Common;
using MediatR;

namespace Application.Use_Cases.ForgotPassword
{
    public class VerifyCodeCommand : IRequest<Result<string>>
    {
        public required string Email { get; set; }
        public required string Code { get; set; }
    }
}
