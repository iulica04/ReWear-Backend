using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.OutfitCommands
{
    public class MatchOutfitItemsCommand : IRequest<Result<List<ClothingItemDTO>>>
    {
        public required Guid UserId { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public List<string> Tags { get; set; } = new();
        public string? Color { get; set; }
        public string? Brand { get; set; }
        public string? Material { get; set; }
        public string? PrintType { get; set; }
        public string? PrintDescription { get; set; }
        public string? Description { get; set; }
    }
}
