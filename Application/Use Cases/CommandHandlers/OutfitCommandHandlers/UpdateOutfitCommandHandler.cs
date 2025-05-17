using Application.Use_Cases.Commands.OutfitCommands;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.OutfitCommandHandlers
{
    public class UpdateOutfitCommandHandler : IRequestHandler<UpdateOutfitCommand>
    {
        private readonly IOutfitRepository repository;
        private readonly IMapper mapper;

        public UpdateOutfitCommandHandler(IOutfitRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task Handle(UpdateOutfitCommand request, CancellationToken cancellationToken)
        {
            var outfit = mapper.Map<Outfit>(request);
            await repository.UpdateAsync(outfit);
        }
    }
}
