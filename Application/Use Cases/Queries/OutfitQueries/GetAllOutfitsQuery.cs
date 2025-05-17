using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Queries.OutfitQueries
{
    public class GetAllOutfitsQuery : IRequest<Result<List<OutfitDTO>>>
    {
    }
}
