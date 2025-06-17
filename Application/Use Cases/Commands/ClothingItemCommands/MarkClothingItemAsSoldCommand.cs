using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.ClothingItemCommands
{
    public class MarkClothingItemAsSoldCommand : IRequest<Result<Unit>>
    {
        public Guid Id { get; set; }
    }
}
