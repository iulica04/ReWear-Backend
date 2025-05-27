using Application.Use_Cases.Commands.FavoriteOutfitCommands;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.FavoriteOutfitCommandHandlers
{
    public class DeleteFavoriteOutfitCommandHandler : IRequestHandler<DeleteFavoriteOutfitCommand, Result<Unit>>
    {
        private readonly IFavoriteOutfitRepository repository;
        public DeleteFavoriteOutfitCommandHandler(IFavoriteOutfitRepository repository)
        {
            this.repository = repository;
        }
        public async Task<Result<Unit>> Handle(DeleteFavoriteOutfitCommand request, CancellationToken cancellationToken)
        {
            var favoriteOutfit = await repository.GetByUserAndOutfitAsync(request.UserId, request.OutfitId);
            if (favoriteOutfit == null)
            {
                return Result<Unit>.Failure("Favorite outfit not found");
            }
            await repository.DeleteAsync(favoriteOutfit.Id);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
