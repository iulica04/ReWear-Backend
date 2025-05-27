using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.FavoriteOutfitCommands
{
    public class DeleteFavoriteOutfitCommand: IRequest<Result<Unit>>
    {
        public required Guid UserId { get; set; }
        public required Guid OutfitId { get; set; }
    }
}
