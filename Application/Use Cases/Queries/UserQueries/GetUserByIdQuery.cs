using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Queries.UserQueries
{
    public class GetUserByIdQuery : IRequest<Result<UserDTO>>
    {
        public Guid Id { get; set; }
    }
}
