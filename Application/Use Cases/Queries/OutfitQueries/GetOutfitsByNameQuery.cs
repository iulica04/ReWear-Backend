using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Queries.OutfitQueries
{
    public class GetOutfitsByNameQuery : IRequest<Result<List<OutfitDTO>>>
    {
        public Guid UserId { get; set; }
        public required string Name { get; set; }
    }
}
