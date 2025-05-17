using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.OutfitCommands
{
    public record DeleteOutfitCommand(Guid Id) : IRequest<Result<Unit>>
    {
    }
}
