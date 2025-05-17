using Application.Services;
using Application.Use_Cases.Commands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;


namespace Application.Use_Cases.CommandHandlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
    {
        private readonly IUserRepository repository;
        private readonly IPasswordHasher passwordHasher;
        private readonly IMapper mapper;

        public CreateUserCommandHandler(IUserRepository repository, IMapper mapper, IPasswordHasher passwordHasher)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.passwordHasher = passwordHasher;
        }

        public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Map the request to a User entity
            var user = mapper.Map<User>(request);
            user.PasswordHash = passwordHasher.HashPassword(request.Password);
            user.Role = "User"; 

            // Save the user to the database
            var result = await repository.AddAsync(user);
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }
            else
            {
                return Result<Guid>.Failure(result.ErrorMessage);
            }
        }
    }
}