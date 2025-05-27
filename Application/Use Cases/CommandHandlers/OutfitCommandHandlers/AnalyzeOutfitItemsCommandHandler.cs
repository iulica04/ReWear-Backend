using Application.Models;
using Application.Services;
using Application.Use_Cases.Commands.OutfitCommands;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.OutfitCommandHandlers
{
    public class AnalyzeOutfitItemsCommandHandler : IRequestHandler<AnalyzeOutfitItemsCommand, Result<List<OutfitItemsAnalysisResult>>>
    {
        private readonly IOutfitService outfitService;
        public AnalyzeOutfitItemsCommandHandler(IOutfitService outfitService)
        {
            this.outfitService = outfitService;
        }
        public async Task<Result<List<OutfitItemsAnalysisResult>>> Handle(AnalyzeOutfitItemsCommand request, CancellationToken cancellationToken)
        {
            var result = await outfitService.AnalyzeClothingItemsAsync(request.ImageFront);
            if (result.IsSuccess)
            {
                return Result<List<OutfitItemsAnalysisResult>>.Success(result.Data);
            }
            else
            {
                return Result<List<OutfitItemsAnalysisResult>>.Failure(result.ErrorMessage);
            }
        }
    }

}
