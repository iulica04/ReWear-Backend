using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.OutfitCommands
{
    public class UpdateOutfitCommand : IRequest<Result<string>>
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public required string Name { get; set; }
        public List<Guid> ClothingItemIds { get; set; } = new();
        public string? Season { get; set; }
        public string? Description { get; set; }
        public byte[]? ImageFront { get; set; }
    }
}
