using Application.Use_Cases.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers
{
    public class UppdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
    {
        private readonly IUserRepository repository;
        private readonly IMapper mapper;
        public UppdateUserCommandHandler(IUserRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = mapper.Map<User>(request);
            await repository.UpdateAsync(user);
        }
    }

}
