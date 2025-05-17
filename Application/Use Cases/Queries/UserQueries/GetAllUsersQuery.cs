using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Queries.UserQueries
{
    public class GetAllUsersQuery : IRequest<Result<List<UserDTO>>>
    {
    }
}
