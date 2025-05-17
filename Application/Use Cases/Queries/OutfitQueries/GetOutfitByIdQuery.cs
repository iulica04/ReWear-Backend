using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Queries.OutfitQueries
{
    public class GetOutfitByIdQuery : IRequest<Result<OutfitDTO>>
    {
        public Guid Id { get; set; }
    }
}
