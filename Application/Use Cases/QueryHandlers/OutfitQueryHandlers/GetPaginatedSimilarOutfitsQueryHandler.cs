using Application.DTOs;
using Application.Services;
using Application.Use_Cases.Queries.OutfitQueries;
using Application.Utils;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Models;
using Domain.Repositories;
using Gridify;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.OutfitQueryHandlers
{
    public class GetPaginatedSimilarOutfitsQueryHandler : IRequestHandler<GetPaginatedSimilarOutfitsQuery, Result<PagedResult<OutfitDTO>>>
    {
        private readonly IOutfitRepository repository;
        private readonly IEmbeddingService embeddingService;

        public GetPaginatedSimilarOutfitsQueryHandler(IOutfitRepository outfitRepository, IEmbeddingService embeddingService)
        {
            this.repository = outfitRepository;
            this.embeddingService = embeddingService;
        }

        public async Task<Result<PagedResult<OutfitDTO>>> Handle(GetPaginatedSimilarOutfitsQuery request, CancellationToken cancellationToken)
        {
            var allOutfits = await repository.GetAllAsync();
            var targetOutfit = await repository.GetByIdAsync(request.Id);
            if (targetOutfit == null)
            {
                return Result<PagedResult<OutfitDTO>>.Failure("Outfit not found");
            }

            var filteredWithSimilarity = allOutfits
            .Where(o => o.Id != request.Id && o.Embedding != null)
            .Select(o => new
            {
                Outfit = o,
                Similarity = embeddingService.ComputeCosineSimilarity(targetOutfit.Embedding!, o.Embedding!)
            })
            .Where(x => x.Similarity >= 0.7)
            .OrderByDescending(x => x.Similarity)
            .ToList();

            var totalCount = filteredWithSimilarity.Count();
            var pagedItems = filteredWithSimilarity.AsQueryable()
                .ApplyPaging(request.Page, request.PageSize);

            var outfitDtos = pagedItems.Select(x => new OutfitDTO
            {
                Id = x.Outfit.Id,
                UserId = x.Outfit.UserId,
                Name = x.Outfit.Name,
                CreatedAt = x.Outfit.CreatedAt,
                Season = x.Outfit.Season,
                Description = x.Outfit.Description,
                ImageUrl = x.Outfit.ImageUrl,
                ClothingItemDTOs = x.Outfit.OutfitClothingItems
             .Where(oci => oci.ClothingItem != null)
             .Select(oci => new ClothingItemDTO
             {
                 Id = oci.ClothingItem.Id,
                 UserId = oci.ClothingItem.UserId,
                 Name = oci.ClothingItem.Name,
                 Category = oci.ClothingItem.Category,
                 Color = oci.ClothingItem.Color,
                 Brand = oci.ClothingItem.Brand,
                 Material = oci.ClothingItem.Material,
                 PrintType = oci.ClothingItem.PrintType,
                 PrintDescription = oci.ClothingItem.PrintDescription,
                 Description = oci.ClothingItem.Description,
                 FrontImageUrl = oci.ClothingItem.FrontImageUrl,
                 BackImageUrl = oci.ClothingItem.BackImageUrl,
                 NumberOfWears = oci.ClothingItem.NumberOfWears,
                 LastWornDate = oci.ClothingItem.LastWornDate
                 // Tags = oci.ClothingItem.Tags.Select(tag => tag.Tag).ToList()
             }).ToList()
            }).ToList();


            var pagedResult = new PagedResult<OutfitDTO>(outfitDtos, totalCount);

            return Result<PagedResult<OutfitDTO>>.Success(pagedResult);


        }
    }
}
