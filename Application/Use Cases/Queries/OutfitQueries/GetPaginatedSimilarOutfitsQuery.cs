using Application.DTOs;
using Application.Utils;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Queries.OutfitQueries
{
    public class GetPaginatedSimilarOutfitsQuery : IRequest<Result<PagedResult<OutfitDTO>>>
    {
        public Guid Id { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
