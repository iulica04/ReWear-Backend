using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class LoginCommand : IRequest<Result<LoginResult>>
    {
        public  required string Email { get; set; }
        public required string Password { get; set; }
    }
}
