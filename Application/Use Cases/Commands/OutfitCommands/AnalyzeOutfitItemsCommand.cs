using Application.Models;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.OutfitCommands
{
    public class AnalyzeOutfitItemsCommand : IRequest<Result<List<OutfitItemsAnalysisResult>>>
    {
         public required byte[] ImageFront { get; set; }
    }
}
