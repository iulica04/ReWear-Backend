using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Queries.FavoriteOutfitQueries
{
    public class GetByIdQuery : IRequest<Result<FavoriteOutfitDTO>>
    {
        public Guid Id { get; set; }
    }
}
