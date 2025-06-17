using Application.Use_Cases.QueryHandlers.OutfitQueryHandlers;
using Application.Use_Cases.Queries.OutfitQueries;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace ReWear.Application.UnitTests.OutfitUnitTests
{
    public class GetAllOutfitsQueryHandlerTests
    {
        private readonly IOutfitRepository repository = Substitute.For<IOutfitRepository>();
        private readonly IMapper mapper = Substitute.For<IMapper>();
        private readonly GetAllOutfitsQueryHandler handler;

        public GetAllOutfitsQueryHandlerTests()
        {
            handler = new GetAllOutfitsQueryHandler(repository, mapper);
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedOutfitDTOs_WhenOutfitsExist()
        {
            // Arrange
            var clothingItem = new ClothingItem
            {
                Id = Guid.Parse("8130ef37-25b4-4633-9350-3e8de9e0f3fd"),
                UserId = Guid.Parse("8ca2225f-4022-459f-a2ad-b70618cd8e9c"),
                Name = "T-Shirt",
                Category = "Top",
                Color = "Red",
                Brand = "Nike",
                Material = "Cotton",
                PrintType = "Logo",
                PrintDescription = "Big Swoosh",
                Description = "Red Nike shirt",
                FrontImageUrl = "front.jpg",
                BackImageUrl = "back.jpg",
                NumberOfWears = 3,
                LastWornDate = DateTime.Today
            };

            var outfit = new Outfit
            {
                Id = Guid.Parse("8c1ae239-734b-4d8d-891d-5e7fd40ea662"),
                UserId = Guid.Parse("90febf99-328c-48ac-bbe3-2303c152a67b"),
                Name = "Casual Outfit",
                CreatedAt = DateTime.UtcNow,
                Season = "Summer",
                Description = "A summer outfit",
                ImageUrl = "outfit.jpg",
                OutfitClothingItems = new List<OutfitClothingItem>
                {
                    new OutfitClothingItem
                    {
                        ClothingItem = clothingItem
                    }
                }
            };

            var outfits = new List<Outfit> { outfit };
            repository.GetAllAsync().Returns(outfits);

            // Act
            var result = await handler.Handle(new GetAllOutfitsQuery(), CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().HaveCount(1);

            var returnedOutfit = result.Data.First();
            returnedOutfit.Name.Should().Be("Casual Outfit");
            returnedOutfit.ClothingItemDTOs.Should().HaveCount(1);
            returnedOutfit.ClothingItemDTOs.First().Name.Should().Be("T-Shirt");
        }

        [Fact]
        public async Task Handle_ShouldReturnOutfitsWithEmptyClothingItemDTOs_WhenNoClothingItemsExist()
        {
            // Arrange
            var outfit = new Outfit
            {
                Id = Guid.Parse("8c1ae239-734b-4d8d-891d-5e7fd40ea662"),
                UserId = Guid.Parse("90febf99-328c-48ac-bbe3-2303c152a67b"),
                Name = "Outfit Without Clothes",
                CreatedAt = DateTime.UtcNow,
                Season = "Winter",
                Description = "Just testing",
                ImageUrl = "noitems.jpg",
                OutfitClothingItems = new List<OutfitClothingItem>() // empty list
            };

            repository.GetAllAsync().Returns(new List<Outfit> { outfit });

            // Act
            var result = await handler.Handle(new GetAllOutfitsQuery(), CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().HaveCount(1);
            result.Data.First().ClothingItemDTOs.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoOutfitsExist()
        {
            // Arrange
            repository.GetAllAsync().Returns(new List<Outfit>());

            // Act
            var result = await handler.Handle(new GetAllOutfitsQuery(), CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEmpty();
        }
    }
}
