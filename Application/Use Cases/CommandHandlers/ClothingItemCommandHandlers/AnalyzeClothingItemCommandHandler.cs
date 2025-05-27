using Application.Models;
using Application.Services;
using Application.Use_Cases.Commands.ClothingItemCommand;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.ClothingItemCommandHandlers
{
    public class AnalyzeClothingItemCommandHandler : IRequestHandler<AnalyzeClothingItemCommand, Result<ImageAnalysisResult>>
    {
        private readonly IClothingItemService clothingItemService;
        public AnalyzeClothingItemCommandHandler(IClothingItemService clothingItemService)
        {
            this.clothingItemService = clothingItemService;
        }
        public async Task<Result<ImageAnalysisResult>> Handle(AnalyzeClothingItemCommand request, CancellationToken cancellationToken)
        {

            var result = await clothingItemService.AnalyzeClothingItemsAsync(request.ImageFront, request.ImageBack);
            if (result.IsSuccess)
            {
                return Result<ImageAnalysisResult>.Success(result.Data);
            }
            else
            {
                return Result<ImageAnalysisResult>.Failure(result.ErrorMessage);
            }
        }
    }

}
