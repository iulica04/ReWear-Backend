using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class LoginCommand : IRequest<Result<string>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
