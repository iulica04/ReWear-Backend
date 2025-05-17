using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.ClothingItemCommand
{
    public class UpdateClothingItemCommand : CreateClothingItemCommand, IRequest
    {
        public Guid Id { get; set; }
    }
}
