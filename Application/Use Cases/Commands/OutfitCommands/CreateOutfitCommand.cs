using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.OutfitCommands
{
    public class CreateOutfitCommand : IRequest<Result<Guid>>
    {
        public Guid UserId { get; set; }
        public required string Name { get; set; }
        public List<Guid> ClothingItemIds { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public string? Season { get; set; }
        public string? Description { get; set; }
        public required string ImageUrl { get; set; }
    }
}
