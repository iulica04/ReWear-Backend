using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Queries.ClothingItemQueries
{
    public class GetAllClothingItemsQuery : IRequest<Result<List<ClothingItemDTO>>>
    {
    }
}
