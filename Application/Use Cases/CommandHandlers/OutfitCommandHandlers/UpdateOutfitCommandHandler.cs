using Application.Services;
using Application.Use_Cases.Commands.OutfitCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.OutfitCommandHandlers
{
    public class UpdateOutfitCommandHandler : IRequestHandler<UpdateOutfitCommand, Result<string>>
    {
        private readonly IOutfitRepository repository;
        private readonly IMapper mapper;
        private readonly IOutfitService outfitService;
        private readonly IClothingItemRepository clothingItemRepository;
        private readonly IEmbeddingService embeddingService;

        public UpdateOutfitCommandHandler(IOutfitRepository repository, IMapper mapper, IOutfitService outfitService, IClothingItemRepository clothingItemRepository, IEmbeddingService embeddingService)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.outfitService = outfitService;
            this.clothingItemRepository = clothingItemRepository;
            this.embeddingService = embeddingService;
        }

        public async Task<Result<string>> Handle(UpdateOutfitCommand request, CancellationToken cancellationToken)
        {
            var outfit = await repository.GetByIdAsync(request.Id);
            if (outfit == null)
            {
                return Result<string>.Failure("Outfit not found");
            }


            string imageUrl = null;
            if (request.ImageFront != null)
            {
                var result = await outfitService.UploadImageAsync(request.ImageFront, request.UserId.ToString(), outfit.Id.ToString(), "Outfit", "Front");
                imageUrl = result.Data;
            }
            else
            {
                imageUrl = outfit.ImageUrl;
            }

            outfit.OutfitClothingItems.Clear();
            foreach (var itemId in request.ClothingItemIds)
            {
                outfit.OutfitClothingItems.Add(new OutfitClothingItem
                {
                    OutfitId = outfit.Id,
                    ClothingItemId = itemId
                });

                // Poți incrementa NumberOfWears dacă vrei
                var item = await clothingItemRepository.GetByIdAsync(itemId);
                if (item != null)
                {
                    item.NumberOfWears++;
                    await clothingItemRepository.UpdateAsync(item);
                }
            }

            outfit.ImageUrl = imageUrl;
            outfit.Name = request.Name;
            outfit.Season = request.Season;
            outfit.Description = request.Description;
            outfit.Embedding = await embeddingService.GetEmbeddingAsync(request.Description!);

            var finalRestul = await repository.UpdateAsync(outfit);
            if(finalRestul.IsSuccess)
            {
                return Result<string>.Success("Outfit updated successfully");
            }
            else
            {
                return Result<string>.Failure(finalRestul.ErrorMessage);
            }
        }
    }
}
