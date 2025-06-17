using Application.Models;
using Application.Services;
using Application.Use_Cases.CommandHandlers.ClothingItemCommandHandlers;
using Application.Use_Cases.Commands.ClothingItemCommand;
using Application.Use_Cases.Commands.OutfitCommands;
using Domain.Common;
using Domain.Entities;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReWear.Application.UnitTests.OutfitUnitTests
{
    public class AnalyzeClothingItemCommandHandlerTests
    {
        private readonly IClothingItemService clothingItemService;
        private readonly AnalyzeClothingItemCommandHandler handler;

        public AnalyzeClothingItemCommandHandlerTests()
        {
            clothingItemService = Substitute.For<IClothingItemService>();
            handler = new AnalyzeClothingItemCommandHandler(clothingItemService);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenServiceReturnsSuccess()
        {
            // Arrange
            var imageFront = new byte[] { 1, 2 };
            var imageBack = new byte[] { 3, 4 };
            var analysisResult = new ImageAnalysisResult();
            var serviceResult = Result<ImageAnalysisResult>.Success(analysisResult);

            clothingItemService.AnalyzeClothingItemsAsync(imageFront, imageBack)
                .Returns(Task.FromResult(serviceResult));

            var command = new AnalyzeClothingItemCommand { ImageFront = imageFront, ImageBack = imageBack };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(analysisResult);
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenServiceReturnsFailure()
        {
            // Arrange
            var imageFront = new byte[] { 5, 6 };
            var imageBack = new byte[] { 7, 8 };
            var errorMessage = "Failed";
            var serviceResult = Result<ImageAnalysisResult>.Failure(errorMessage);

            clothingItemService.AnalyzeClothingItemsAsync(imageFront, imageBack)
                .Returns(Task.FromResult(serviceResult));

            var command = new AnalyzeClothingItemCommand { ImageFront = imageFront, ImageBack = imageBack };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.ErrorMessage.Should().Be(errorMessage);
        }

        [Fact]
        public async Task Handle_ShouldCallService_WithGivenImages()
        {
            // Arrange
            var imageFront = new byte[] { 9 };
            var imageBack = new byte[] { 10 };
            var serviceResult = Result<ImageAnalysisResult>.Success(new ImageAnalysisResult());
            clothingItemService.AnalyzeClothingItemsAsync(imageFront, imageBack)
                .Returns(Task.FromResult(serviceResult));

            var command = new AnalyzeClothingItemCommand { ImageFront = imageFront, ImageBack = imageBack };

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            await clothingItemService.Received(1).AnalyzeClothingItemsAsync(imageFront, imageBack);
        }
    }
}