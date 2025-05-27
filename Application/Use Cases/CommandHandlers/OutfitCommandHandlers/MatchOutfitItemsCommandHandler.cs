using Application.DTOs;
using Application.Services;
using Application.Use_Cases.Commands.OutfitCommands;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;
using AutoMapper;
using Newtonsoft.Json;

namespace Application.Use_Cases.CommandHandlers.OutfitCommandHandlers
{
    public class MatchOutfitItemsCommandHandler : IRequestHandler<MatchOutfitItemsCommand, Result<List<ClothingItemDTO>>>
    {
        private readonly IOutfitRepository outfitRepository;
        private readonly IClothingItemRepository clothingItemRepository;
        private readonly IEmbeddingService embeddingService;
        private readonly IMapper mapper;

        public MatchOutfitItemsCommandHandler(
            IOutfitRepository outfitRepository,
            IClothingItemRepository clothingItemRepository,
            IEmbeddingService embeddingService,
            IMapper mapper)
        {
            this.outfitRepository = outfitRepository;
            this.clothingItemRepository = clothingItemRepository;
            this.embeddingService = embeddingService;
            this.mapper = mapper;
        }

        public async Task<Result<List<ClothingItemDTO>>> Handle(MatchOutfitItemsCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Description))
                return Result<List<ClothingItemDTO>>.Failure("Description cannot be null or empty.");

            var outfitEmbedding = await embeddingService.GetEmbeddingAsync(request.Description);

            var allItems = await clothingItemRepository.GetAllAsync();
            var itemsWithEmbedding = allItems
                .Where(x => x.Embedding != null && x.Embedding.Length > 0 && x.UserId == request.UserId)
                .ToList();

            const float similarityThreshold = 0.7f;

            var top3 = itemsWithEmbedding
                .Select(item => new
                {
                    Item = item,
                    Similarity = embeddingService.ComputeCosineSimilarity(outfitEmbedding, item.Embedding!)
                })    
                .Where(x => x.Similarity >= similarityThreshold)
                .OrderByDescending(x => x.Similarity)
                .Take(3)
                .Select(x => x.Item)
                .ToList();

            var dtos = mapper.Map<List<ClothingItemDTO>>(top3);

            return Result<List<ClothingItemDTO>>.Success(dtos);
        }
    }
}