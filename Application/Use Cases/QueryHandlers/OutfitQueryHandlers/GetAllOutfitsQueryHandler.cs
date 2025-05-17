using Application.DTOs;
using Application.Use_Cases.Queries.OutfitQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.OutfitQueryHandlers
{
    public class GetAllOutfitsQueryHandler : IRequestHandler<GetAllOutfitsQuery, Result<List<OutfitDTO>>>
    {
        private readonly IOutfitRepository repository;
        private readonly IMapper mapper;

        public GetAllOutfitsQueryHandler(IOutfitRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<Result<List<OutfitDTO>>> Handle(GetAllOutfitsQuery request, CancellationToken cancellationToken)
        {
            var outfits = await repository.GetAllAsync();
            return Result<List<OutfitDTO>>.Success(mapper.Map<List<OutfitDTO>>(outfits));
        }
    }
}
