using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Queries.UserQueries
{
    public class CheckUserExistenceQuery : IRequest<Result<bool>>
    {
        public required string EmailOrUsername { get; set; }
    }
  
}
