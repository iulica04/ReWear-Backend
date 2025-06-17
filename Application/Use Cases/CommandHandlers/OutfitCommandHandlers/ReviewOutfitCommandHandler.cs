using Application.Models;
using Application.Services;
using Application.Use_Cases.Commands.OutfitCommands;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.OutfitCommandHandlers
{
    public class ReviewOutfitCommandHandler : IRequestHandler<ReviewOutfitCommand, Result<ReviewOutfitResult>>
    {
        private readonly IWeatherServices weatherServices;
        private readonly IOutfitService outfitService;

        public ReviewOutfitCommandHandler(IWeatherServices weatherServices, IOutfitService outfitService)
        {
            this.weatherServices = weatherServices;
            this.outfitService = outfitService;
        }
        public async Task<Result<ReviewOutfitResult>> Handle(ReviewOutfitCommand request, CancellationToken cancellationToken)
        {
            var weather = await weatherServices.GetWeatherAsync(request.Lon, request.Lat);
            var outfitReview = await outfitService.ReviewOutfit(weather, request.Image, request.UserContext);
            if (outfitReview.IsSuccess)
            {
                return Result<ReviewOutfitResult>.Success(outfitReview.Data);
            }
            else
            {
                return Result<ReviewOutfitResult>.Failure(outfitReview.ErrorMessage);
            }

        }
    }
}
