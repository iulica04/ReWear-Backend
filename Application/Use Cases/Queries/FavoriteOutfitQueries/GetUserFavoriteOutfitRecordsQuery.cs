using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Queries.OutfitQueries
{
    public class GetUserFavoriteOutfitRecordsQuery : IRequest<Result<List<FavoriteOutfitDTO>>>
    {
        public required Guid UserId { get; set; }
    }
}
