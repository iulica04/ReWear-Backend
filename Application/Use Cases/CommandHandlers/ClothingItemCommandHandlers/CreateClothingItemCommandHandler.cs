using Application.Services;
using Application.Use_Cases.Commands.ClothingItemCommand;
using Domain.Common;
using Domain.Entities;
using Domain.Models;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.ClothingItemCommandHandlers
{
    public class CreateClothingItemCommandHandler : IRequestHandler<CreateClothingItemCommand, Result<Guid>>
    {
        private readonly IClothingItemRepository clothingItemRepository;
        private readonly IClothingItemService clothingItemService;
        private readonly IEmbeddingService embeddingService;

        public CreateClothingItemCommandHandler(IClothingItemRepository clothingItemRepository, IClothingItemService clothingItemService, IEmbeddingService embeddingService)
        {
            this.clothingItemRepository = clothingItemRepository;
            this.clothingItemService = clothingItemService;
            this.embeddingService = embeddingService;
        }
        public async Task<Result<Guid>> Handle(CreateClothingItemCommand request, CancellationToken cancellationToken)
        {
            var clothingItemId = Guid.NewGuid();
            var bucketNameImageFront = await clothingItemService.UploadImageAsync(request.ImageFront, request.UserId.ToString(), clothingItemId.ToString(), "ClothingItem", "Front");
            string? bucketNameImageBack = null;
            if (request.ImageBack != null)
            {
                var result = await clothingItemService.UploadImageAsync(
                    request.ImageBack,
                    request.UserId.ToString(),
                    clothingItemId.ToString(),
                    "ClothingItem",
                    "Back");

                bucketNameImageBack = result.Data;
            }

            var clothingTags = request.Tags.Select(tag => new ClothingTag
            {
                Tag = tag
            }).ToList();


            var clothingItem = new ClothingItem
            {
                Id = clothingItemId,
                UserId = request.UserId,
                Name = request.Name,
                Category = request.Category,
                Tags = clothingTags,
                Color = request.Color,
                Brand = request.Brand,
                Material = request.Material,
                PrintType = request.PrintType,
                PrintDescription = request.PrintDescription,
                Description = request.Description,
                FrontImageUrl = bucketNameImageFront.Data,
                BackImageUrl = bucketNameImageBack,
                Embedding = await embeddingService.GetEmbeddingAsync(request.Description),
                NumberOfWears = 0,
            };

            try
            {
                var result = await clothingItemRepository.AddAsync(clothingItem);
                if (result.IsSuccess)
                    return Result<Guid>.Success(result.Data);
                return Result<Guid>.Failure(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                // Log ex.ToString() sau ex.InnerException?.Message
                return Result<Guid>.Failure("Error adding clothing item: " + ex.ToString());
            }
        }
    }

}
