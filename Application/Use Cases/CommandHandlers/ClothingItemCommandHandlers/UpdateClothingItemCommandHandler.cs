using Application.Services;
using Application.Use_Cases.Commands.ClothingItemCommand;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Models;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.ClothingItemCommandHandlers
{
    public class UpdateClothingItemCommandHandler : IRequestHandler<UpdateClothingItemCommand, Result<string>>
    {
        private readonly IClothingItemRepository clothingItemRepository;
        private readonly IMapper mapper;
        private readonly IClothingItemService clothingItemService;


        public UpdateClothingItemCommandHandler(IClothingItemRepository clothingItemRepository, IMapper mapper, IClothingItemService clothingItemService)
        {
            this.clothingItemRepository = clothingItemRepository;
            this.mapper = mapper;
            this.clothingItemService = clothingItemService;
        }

        public async Task<Result<string>> Handle(UpdateClothingItemCommand request, CancellationToken cancellationToken)
        {
            var clothingItem = await clothingItemRepository.GetByIdAsync(request.Id);
            if (clothingItem == null)
            {
                return Result<string>.Failure("Clothing item not found");
            }


            string? bucketNameImageFront = null;
            if (request.ImageFront != null)
            {
                var result = await clothingItemService.UploadImageAsync(request.ImageFront, request.UserId.ToString(), clothingItem.Id.ToString(), "ClothingItem", "Front");
                bucketNameImageFront = result.Data;
            }
            else
            {
                bucketNameImageFront = clothingItem.FrontImageUrl;
            }
            string? bucketNameImageBack = null;
            if (request.ImageBack != null)
            {
                var result = await clothingItemService.UploadImageAsync(
                    request.ImageBack,
                    request.UserId.ToString(),
                    clothingItem.Id.ToString(),
                    "ClothingItem",
                    "Back");

                bucketNameImageBack = result.Data;
            }
            else
            {
                bucketNameImageBack = clothingItem.BackImageUrl;
            }


            var clothingTags = request.Tags.Select(tag => new ClothingTag
            {
                Tag = tag
            }).ToList();

            clothingItem.Tags = clothingTags;
            clothingItem.Name = request.Name;
            clothingItem.Category = request.Category;
            clothingItem.Color = request.Color;
            clothingItem.Brand = request.Brand;
            clothingItem.Material = request.Material;
            clothingItem.PrintType = request.PrintType;
            clothingItem.PrintDescription = request.PrintDescription;
            clothingItem.Description = request.Description;
            clothingItem.FrontImageUrl = bucketNameImageFront;
            clothingItem.BackImageUrl = bucketNameImageBack;
 
            var final = await clothingItemRepository.UpdateAsync(clothingItem);
            if(final.IsSuccess)
            {
                return Result<string>.Success("Clothing item updated successfully");
            }
            else
            {
                return Result<string>.Failure(final.ErrorMessage);
            }
        }
    }
}
