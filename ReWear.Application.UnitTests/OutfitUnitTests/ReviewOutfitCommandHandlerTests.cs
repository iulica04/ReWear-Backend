using Application.Models;
using Application.Services;
using Application.Use_Cases.CommandHandlers.OutfitCommandHandlers;
using Application.Use_Cases.Commands.OutfitCommands;
using Domain.Common;
using FluentAssertions;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReWear.Application.UnitTests.OutfitUnitTests
{
    public class ReviewOutfitCommandHandlerTests
    {
        private readonly IWeatherServices weatherServices;
        private readonly IOutfitService outfitService;
        private readonly ReviewOutfitCommandHandler handler;

        public ReviewOutfitCommandHandlerTests()
        {
            weatherServices = Substitute.For<IWeatherServices>();
            outfitService = Substitute.For<IOutfitService>();
            handler = new ReviewOutfitCommandHandler(weatherServices, outfitService);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenReviewIsSuccessful()
        {
            var lon = "10.0";
            var lat = "20.0";
            var image = new byte[] { 1, 2, 3 };
            var userContext = "context";
            var weather = "sunny";
            var reviewResult = new ReviewOutfitResult
            {
                Review = "Good",
                Suggestions = "None",
                OverallAdvice = "Wear it"
            };
            var reviewServiceResult = Result<ReviewOutfitResult>.Success(reviewResult);

            weatherServices.GetWeatherAsync(lon, lat).Returns(Task.FromResult(weather));
            outfitService.ReviewOutfit(weather, image, userContext).Returns(Task.FromResult(reviewServiceResult));

            var command = new ReviewOutfitCommand
            {
                Lon = lon,
                Lat = lat,
                Image = image,
                UserContext = userContext
            };

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(reviewResult);
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenReviewFails()
        {
            var lon = "10.0";
            var lat = "20.0";
            var image = new byte[] { 1, 2, 3 };
            var userContext = "context";
            var weather = "rainy";
            var errorMessage = "Review failed";
            var reviewServiceResult = Result<ReviewOutfitResult>.Failure(errorMessage);

            weatherServices.GetWeatherAsync(lon, lat).Returns(Task.FromResult(weather));
            outfitService.ReviewOutfit(weather, image, userContext).Returns(Task.FromResult(reviewServiceResult));

            var command = new ReviewOutfitCommand
            {
                Lon = lon,
                Lat = lat,
                Image = image,
                UserContext = userContext
            };

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.ErrorMessage.Should().Be(errorMessage);
        }

        [Fact]
        public async Task Handle_ShouldCallWeatherAndOutfitService_WithCorrectParameters()
        {
            var lon = "1.1";
            var lat = "2.2";
            var image = new byte[] { 9, 8, 7 };
            var userContext = "ctx";
            var weather = "cloudy";
            var reviewServiceResult = Result<ReviewOutfitResult>.Success(new ReviewOutfitResult());

            weatherServices.GetWeatherAsync(lon, lat).Returns(Task.FromResult(weather));
            outfitService.ReviewOutfit(weather, image, userContext).Returns(Task.FromResult(reviewServiceResult));

            var command = new ReviewOutfitCommand
            {
                Lon = lon,
                Lat = lat,
                Image = image,
                UserContext = userContext
            };

            await handler.Handle(command, CancellationToken.None);

            await weatherServices.Received(1).GetWeatherAsync(lon, lat);
            await outfitService.Received(1).ReviewOutfit(weather, image, userContext);
        }

        [Fact]
        public void ReviewOutfitResult_ShouldAllowNullProperties()
        {
            var result = new ReviewOutfitResult();
            result.Review.Should().BeNull();
            result.Suggestions.Should().BeNull();
            result.OverallAdvice.Should().BeNull();
        }

        [Fact]
        public void ReviewOutfitResult_ShouldAssignProperties()
        {
            var result = new ReviewOutfitResult
            {
                Review = "review",
                Suggestions = "suggest",
                OverallAdvice = "advice"
            };

            result.Review.Should().Be("review");
            result.Suggestions.Should().Be("suggest");
            result.OverallAdvice.Should().Be("advice");
        }
    }
}