using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Queries.ClothingItemQueries
{
    public class GetClothingItemByIdQuery : IRequest<Result<ClothingItemDTO>>
    {
        public Guid Id { get; set; }
    }
}
