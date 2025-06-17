using Application.Models;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.OutfitCommands
{
    public class ReviewOutfitCommand : IRequest<Result<ReviewOutfitResult>>
    {
        public required string Lon { get; set; }
        public required string Lat { get; set; }
        public required byte[] Image { get; set; }
        public string? UserContext { get; set; } = null!;
    }
}
