using Application.DTOs;
using Application.Use_Cases.Queries.UserQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.UserQueryHandlers
{
    public class CheckUserExistenceQueryHandler : IRequestHandler<CheckUserExistenceQuery, Result<bool>>
    {
        private readonly IUserRepository repository;
        private readonly IMapper mapper;

        public CheckUserExistenceQueryHandler(IUserRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<Result<bool>> Handle(CheckUserExistenceQuery request, CancellationToken cancellationToken)
        {
            var user = await repository.GetByEmailAsync(request.EmailOrUsername);
            if (user != null)
            {
                return Result<bool>.Success(true);
            }
            else
            {
                user = await repository.GetByUserNameAsync(request.EmailOrUsername);
                if (user != null)
                {
                    return Result<bool>.Success(true);
                }
                else
                {
                    return Result<bool>.Failure("User doesn't exists.");
                }
            }
        }
    }
}
