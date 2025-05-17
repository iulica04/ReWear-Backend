using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.ClothingItemCommand
{
    public class CreateClothingItemCommand : IRequest<Result<Guid>>
    {
        public Guid UserId { get; set; }
        public required string Name { get; set; }
        public required string Category { get; set; }
        public List<string> Tags { get; set; } = new();
        public required string Color { get; set; }
        public required string Brand { get; set; }
        public required string Material { get; set; }
        public string? PrintType { get; set; }
        public string? PrintDescription { get; set; }
        public string? Description { get; set; }
        public required byte[] ImageFront { get; set; } 
        public required byte[] ImageBack { get; set; } 

    }
}
