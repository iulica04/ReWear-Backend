using Application.Models;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.OutfitCommands
{
    public class AnalyzeOutfitCommand : IRequest<Result<OutfitAnalysisResult>>
    {
        public required byte[] ImageFront { get; set; }
    }
}
