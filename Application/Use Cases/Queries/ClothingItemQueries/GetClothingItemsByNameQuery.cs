using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Queries.ClothingItemQueries
{
    public class GetClothingItemsByNameQuery : IRequest<Result<List<ClothingItemDTO>>>
    {
        public Guid UserId { get; set; }
        public required string Name { get; set; }
    }
}
