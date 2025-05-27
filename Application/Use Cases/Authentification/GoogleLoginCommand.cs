using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class GoogleLoginCommand : IRequest<Result<LoginResult>>
    {
        public required string IdToken { get; set; }
    }
}
