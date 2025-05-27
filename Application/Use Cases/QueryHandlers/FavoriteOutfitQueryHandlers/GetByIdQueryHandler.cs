using Application.DTOs;
using Application.Use_Cases.Queries.FavoriteOutfitQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.FavoriteOutfitQueryHandlers
{
    public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, Result<FavoriteOutfitDTO>>
    {
        private readonly IMapper mapper;
        private readonly IFavoriteOutfitRepository repository;

        public GetByIdQueryHandler(IMapper mapper, IFavoriteOutfitRepository repository)
        {
            this.mapper = mapper;
            this.repository = repository;
        }
        public async Task<Result<FavoriteOutfitDTO>> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            var favoriteOutfit = await repository.GetByIdAsync(request.Id);
            if (favoriteOutfit == null)
            {
                return Result<FavoriteOutfitDTO>.Failure("Favorite outfit not found");
            }
            var favoriteOutfitDto = new FavoriteOutfitDTO
            {
                Id = favoriteOutfit.Id,
                UserId = favoriteOutfit.UserId,
                OutfitId = favoriteOutfit.OutfitId,
            };
            return Result<FavoriteOutfitDTO>.Success(favoriteOutfitDto);
        }
    }
}
