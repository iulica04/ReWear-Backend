using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands
{
    public class CreateUserCommand : IRequest<Result<Guid>>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public  string? Password { get; set; }
        public string Role { get; set; } = "User"; // Default role
        public string LoginProvider { get; set; } = "local"; // Default login provider
    }
}
