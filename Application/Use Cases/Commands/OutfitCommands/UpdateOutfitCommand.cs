using MediatR;

namespace Application.Use_Cases.Commands.OutfitCommands
{
    public class UpdateOutfitCommand : CreateOutfitCommand, IRequest
    {
        public Guid Id { get; set; }
    }
}
