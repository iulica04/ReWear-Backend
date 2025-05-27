using Application.DTOs;
using Application.Use_Cases.Queries.OutfitQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.FavoriteOutfitQueryHandlers
{
    public class GetUserFavoriteOutfitRecordsQueryHandler : IRequestHandler<GetUserFavoriteOutfitRecordsQuery, Result<List<FavoriteOutfitDTO>>>
    {
        private readonly IFavoriteOutfitRepository repository;
        private readonly IMapper mapper;

        public GetUserFavoriteOutfitRecordsQueryHandler(IFavoriteOutfitRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<Result<List<FavoriteOutfitDTO>>> Handle(GetUserFavoriteOutfitRecordsQuery request, CancellationToken cancellationToken)
        {
            var favoriteOutfits = await repository.GetAllByUserIdAsync(request.UserId);
            return Result<List<FavoriteOutfitDTO>>.Success(mapper.Map<List<FavoriteOutfitDTO>>(favoriteOutfits));

        }
    }
}
