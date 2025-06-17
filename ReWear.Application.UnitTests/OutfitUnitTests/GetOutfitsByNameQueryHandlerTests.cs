using Application.DTOs;
using Application.Use_Cases.Queries.OutfitQueries;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReWear.Application.UnitTests.OutfitUnitTests
{
    public class GetOutfitsByNameQueryHandlerTests
    {
        private readonly IOutfitRepository outfitRepository;
        private readonly IMapper mapper;
        private readonly GetOutfitsByNameQueryHandler handler;

        public GetOutfitsByNameQueryHandlerTests()
        {
            this.outfitRepository = Substitute.For<IOutfitRepository>();
            this.mapper = Substitute.For<IMapper>();
            this.handler = new GetOutfitsByNameQueryHandler(outfitRepository, mapper);
        }

        [Fact]
        public async Task Handle_ShouldReturnMatchingOutfits_ForGivenUserIdAndName()
        {
            // Arrange
            var userId = Guid.Parse("bcac92b0-6796-400b-a494-72fb696d34cf");
            var now = DateTime.UtcNow;

            var outfits = new List<Outfit>
            {
                new Outfit { Id = Guid.Parse("c5b1935c-bf64-4bef-a3fb-2d0a7b64eba1"), UserId = userId, Name = "Summer Outfit", ImageUrl = "", CreatedAt = now },
                new Outfit { Id = Guid.Parse("8130ef37-25b4-4633-9350-3e8de9e0f3fd"), UserId = userId, Name = "summer vibes", ImageUrl = "", CreatedAt = now },
                new Outfit { Id = Guid.Parse("eacb468e-00ec-423a-afe6-70cfecbe2684"), UserId = userId, Name = "Winter Outfit", ImageUrl = "", CreatedAt = now },
                new Outfit { Id = Guid.Parse("796bf470-a8ae-4089-9618-aea0cd57335e"), UserId = Guid.Parse("828a3bab-28be-4b31-b5d6-c81439250e63"), Name = "Summer Outfit", ImageUrl = "", CreatedAt = now } // Different user
            };

            outfitRepository.GetAllAsync().Returns(outfits);

            var expectedOutfits = outfits
                .Where(o => o.UserId == userId && o.Name.Contains("summer", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var expectedDtos = expectedOutfits
                .Select(o => new OutfitDTO
                {
                    Id = o.Id,
                    Name = o.Name,
                    UserId = o.UserId,
                    ImageUrl = o.ImageUrl,
                    CreatedAt = o.CreatedAt
                })
                .ToList();

            mapper.Map<List<OutfitDTO>>(Arg.Is<List<Outfit>>(x => x.SequenceEqual(expectedOutfits)))
                .Returns(expectedDtos);

            var query = new GetOutfitsByNameQuery { UserId = userId, Name = "summer" };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.Data.Should().BeEquivalentTo(expectedDtos);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoOutfitMatches()
        {
            // Arrange
            var userId = Guid.Parse("8130ef37-25b4-4633-9350-3e8de9e0f3fd");
            var now = DateTime.UtcNow;

            var outfits = new List<Outfit>
            {
                new Outfit { Id = Guid.Parse("5b5b20e4-c51e-4096-9320-7e6d2f79c3b4"), UserId = userId, Name = "Winter Look", ImageUrl = "", CreatedAt = now },
                new Outfit { Id = Guid.Parse("05d8095f-afbd-4859-bae3-cb82b7593a9a"), UserId = userId, Name = "Rainy Day", ImageUrl = "", CreatedAt = now }
            };

            outfitRepository.GetAllAsync().Returns(outfits);
            mapper.Map<List<OutfitDTO>>(Arg.Any<List<Outfit>>()).Returns(new List<OutfitDTO>());

            var query = new GetOutfitsByNameQuery { UserId = userId, Name = "summer" };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldIgnoreOutfitsWithNullOrEmptyName()
        {
            // Arrange
            var userId = Guid.Parse("8130ef37-25b4-4633-9350-3e8de9e0f3fd");
            var now = DateTime.UtcNow;

            var outfits = new List<Outfit>
            {
                new Outfit { Id = Guid.Parse("17918f14-d051-4b68-9cd6-c3aa0e676b93"), UserId = userId, Name = null!, ImageUrl = "", CreatedAt = now },
                new Outfit { Id = Guid.Parse("547d7880-a4c9-4786-a646-8461fc457a76"), UserId = userId, Name = "", ImageUrl = "", CreatedAt = now },
                new Outfit { Id = Guid.Parse("29555045-570f-4821-be10-c149f44009cd"), UserId = userId, Name = "Valid Name", ImageUrl = "", CreatedAt = now }
            };

            outfitRepository.GetAllAsync().Returns(outfits);

            var expectedDtos = new List<OutfitDTO>
            {
                new OutfitDTO
                {
                    Id = outfits[2].Id,
                    Name = outfits[2].Name,
                    UserId = outfits[2].UserId,
                    ImageUrl = outfits[2].ImageUrl,
                    CreatedAt = outfits[2].CreatedAt
                }
            };

            mapper.Map<List<OutfitDTO>>(Arg.Is<List<Outfit>>(x => x.Count == 1 && x[0].Id == outfits[2].Id))
                .Returns(expectedDtos);

            var query = new GetOutfitsByNameQuery { UserId = userId, Name = "valid" };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().HaveCount(1);
            result.Data.First().Name.Should().Be("Valid Name");
        }
    }
}
