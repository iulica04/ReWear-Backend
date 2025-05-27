using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands
{
    public class UpdateUserCommand : IRequest<Result<string>>
    {
        public Guid UserId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public string? Password { get; set; }
        public string Role { get; set; } = "User";
        public string LoginProvider { get; set; } = "local";
        public Guid Id { get; set; }
    }
    
}
