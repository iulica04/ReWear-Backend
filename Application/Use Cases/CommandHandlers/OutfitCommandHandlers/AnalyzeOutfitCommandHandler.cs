using Application.Models;
using Application.Services;
using Application.Use_Cases.Commands.OutfitCommands;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.OutfitCommandHandlers
{
    public class AnalyzeOutfitCommandHandler : IRequestHandler<AnalyzeOutfitCommand, Result<OutfitAnalysisResult>>
    {
        private readonly IOutfitService outfitService;
        public AnalyzeOutfitCommandHandler(IOutfitService outfitService)
        {
            this.outfitService = outfitService;
        }
        public async Task<Result<OutfitAnalysisResult>> Handle(AnalyzeOutfitCommand request, CancellationToken cancellationToken)
        {
            var result = await outfitService.AnalyzeOutfitAsync(request.ImageFront);
            if (result.IsSuccess)
            {
                return Result<OutfitAnalysisResult>.Success(result.Data);
            }
            else
            {
                return Result<OutfitAnalysisResult>.Failure(result.ErrorMessage);
            }
        }
    }

}
