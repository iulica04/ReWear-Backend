using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Queries.ClothingItemQueries
{
    public class GetClothingItemsAvaibleForSaleQuery : IRequest<Result<List<ClothingItemDTO>>>
    {
        public Guid UserId { get; set; }
    }
}
