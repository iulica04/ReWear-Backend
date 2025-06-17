using Application.Services;
using Application.Use_Cases.Commands.OutfitCommands;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.OutfitCommandHandlers
{
    public class CreateOutfitCommandHandler : IRequestHandler<CreateOutfitCommand, Result<Guid>>
    {
        private readonly IOutfitRepository outfitRepository;
        private readonly IClothingItemRepository clothingItemRepository;
        private readonly IOutfitService outfitService;
        private readonly IEmbeddingService embeddingService;

        public CreateOutfitCommandHandler(IOutfitRepository outfitRepository, IClothingItemRepository clothingItemRepository, IOutfitService outfitService, IEmbeddingService embeddingService)
        {
            this.outfitRepository = outfitRepository;
            this.clothingItemRepository = clothingItemRepository;
            this.outfitService = outfitService;
            this.embeddingService = embeddingService;
        }
        public async Task<Result<Guid>> Handle(CreateOutfitCommand request, CancellationToken cancellationToken)
        {
            var outfitId = Guid.NewGuid();
            var bucketNameImageFront = await outfitService.UploadImageAsync(
                request.ImageFront, request.UserId.ToString(), outfitId.ToString(), "Outfit", "Front");

            var outfitClothingItems = new List<OutfitClothingItem>();
            foreach (var itemId in request.ClothingItemIds)
            {
                var item = await clothingItemRepository.GetByIdAsync(itemId);
                if (item != null)
                {
                    outfitClothingItems.Add(new OutfitClothingItem
                    {
                        OutfitId = outfitId,
                        ClothingItemId = item.Id
                    });
                    item.NumberOfWears++;
                    item.LastWornDate = DateTime.UtcNow;
                    await clothingItemRepository.UpdateAsync(item);
                }
            }

            if (request.Description == null)
                request.Description = string.Empty;

            var outfit = new Outfit
            {
                Id = outfitId,
                UserId = request.UserId,
                Name = request.Name,
                ImageUrl = bucketNameImageFront.Data,
                Description = request.Description,
                Season = request.Season,
                CreatedAt = DateTime.UtcNow,
                OutfitClothingItems = outfitClothingItems,
                Embedding = await embeddingService.GetEmbeddingAsync(request.Description)
            };

            var result = await outfitRepository.AddAsync(outfit);
            if (result.IsSuccess)
                return Result<Guid>.Success(result.Data);
            return Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}
