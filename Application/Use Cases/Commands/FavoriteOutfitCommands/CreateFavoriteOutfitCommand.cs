using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.FavoriteOutfitCommands
{
    public class CreateFavoriteOutfitCommand : IRequest<Result<Guid>>
    {
        public required Guid UserId { get; set; }
        public required Guid OutfitId { get; set; }
    }
}
