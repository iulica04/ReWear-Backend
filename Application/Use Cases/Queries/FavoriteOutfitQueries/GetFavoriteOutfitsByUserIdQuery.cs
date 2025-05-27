using Application.DTOs;
using Application.Utils;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Queries.FavoriteOutfitQueries
{
    public class GetFavoriteOutfitsByUserIdQuery : IRequest<Result<PagedResult<OutfitDTO>>>
    {
        public Guid UserId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
