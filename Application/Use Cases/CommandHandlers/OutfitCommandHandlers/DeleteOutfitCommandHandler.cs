using Application.Use_Cases.Commands.OutfitCommands;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.OutfitCommandHandlers
{
    public class DeleteOutfitCommandHandler : IRequestHandler<DeleteOutfitCommand, Result<Unit>>
    {
        private readonly IOutfitRepository repository;

        public DeleteOutfitCommandHandler(IOutfitRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<Unit>> Handle(DeleteOutfitCommand request, CancellationToken cancellationToken)
        {
            var outfit = await repository.GetByIdAsync(request.Id);
            if (outfit == null)
            {
                return Result<Unit>.Failure("Outfit not found");
            }
            await repository.DeleteAsync(outfit.Id);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
