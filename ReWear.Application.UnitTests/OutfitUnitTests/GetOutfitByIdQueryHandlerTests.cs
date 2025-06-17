using Application.DTOs;
using Application.Use_Cases.QueryHandlers.OutfitQueryHandlers;
using Application.Use_Cases.Queries.OutfitQueries;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace ReWear.Application.UnitTests.OutfitUnitTests
{
    public class GetOutfitByIdQueryHandlerTests
    {
        private readonly IOutfitRepository repository = Substitute.For<IOutfitRepository>();
        private readonly IMapper mapper = Substitute.For<IMapper>();
        private readonly GetOutfitByIdQueryHandler handler;

        public GetOutfitByIdQueryHandlerTests()
        {
            handler = new GetOutfitByIdQueryHandler(repository, mapper);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessWithMappedDto_WhenOutfitExists()
        {
            // Arrange
            var outfitId = Guid.Parse("8ca2225f-4022-459f-a2ad-b70618cd8e9c");
            var outfit = new Outfit
            {
                Id = outfitId,
                UserId = Guid.Parse("5fd558df-d3d2-4ecc-8c29-202d88e92680"),
                Name = "Summer Fit",
                CreatedAt = DateTime.UtcNow,
                Season = "Summer",
                Description = "Light and breezy",
                ImageUrl = "https://image.url",
                OutfitClothingItems = new List<OutfitClothingItem>
                {
                    new OutfitClothingItem
                    {
                        ClothingItem = new ClothingItem
                        {
                            Id = Guid.Parse("8130ef37-25b4-4633-9350-3e8de9e0f3fd"),
                            UserId = Guid.Parse("8130ef37-25b4-4633-9350-3e8de9e0f3fd"),
                            Name = "T-Shirt",
                            Category = "Top",
                            Color = "White",
                            Brand = "H&M",
                            Material = "Cotton",
                            PrintType = "None",
                            PrintDescription = null,
                            Description = "Plain white tee",
                            FrontImageUrl = "front.jpg",
                            BackImageUrl = "back.jpg",
                            NumberOfWears = 5,
                            LastWornDate = DateTime.Today
                        }
                    }
                }
            };

            var expectedDto = new OutfitDTO
            {
                Id = outfit.Id,
                UserId = outfit.UserId,
                Name = outfit.Name,
                CreatedAt = outfit.CreatedAt,
                Season = outfit.Season,
                Description = outfit.Description,
                ImageUrl = outfit.ImageUrl,
                ClothingItemDTOs = new List<ClothingItemDTO>
                {
                    new ClothingItemDTO
                    {
                        Id = outfit.OutfitClothingItems[0].ClothingItem.Id,
                        UserId = outfit.OutfitClothingItems[0].ClothingItem.UserId,
                        Name = "T-Shirt",
                        Category = "Top",
                        Color = "White",
                        Brand = "H&M",
                        Material = "Cotton",
                        PrintType = "None",
                        PrintDescription = null,
                        Description = "Plain white tee",
                        FrontImageUrl = "front.jpg",
                        BackImageUrl = "back.jpg",
                        NumberOfWears = 5,
                        LastWornDate = DateTime.Today
                    }
                }
            };

            repository.GetByIdAsync(outfitId).Returns(outfit);
            mapper.Map<OutfitDTO>(outfit).Returns(expectedDto);

            var query = new GetOutfitByIdQuery { Id = outfitId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(expectedDto);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenOutfitNotFound()
        {
            // Arrange
            var outfitId = Guid.Parse("29555045-570f-4821-be10-c149f44009cd");
            repository.GetByIdAsync(outfitId).Returns((Outfit?)null);

            var query = new GetOutfitByIdQuery { Id = outfitId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Outfit not found");
        }
    }
}
