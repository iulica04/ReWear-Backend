using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.ClothingItemCommand
{
    public record DeleteClothingItemCommand(Guid Id) : IRequest<Result<Unit>>
    {
    }
}
