using Application.DTOs;
using Application.Use_Cases.Queries.OutfitQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.OutfitQueryHandlers
{
    public class GetOutfitByIdQueryHandler : IRequestHandler<GetOutfitByIdQuery, Result<OutfitDTO>>
    {
        private readonly IOutfitRepository repository;
        private readonly IMapper mapper;

        public GetOutfitByIdQueryHandler(IOutfitRepository outfitRepository, IMapper mapper)
        {
            this.repository = outfitRepository;
            this.mapper = mapper;
        }
        public async Task<Result<OutfitDTO>> Handle(GetOutfitByIdQuery request, CancellationToken cancellationToken)
        {
            var outfit = await repository.GetByIdAsync(request.Id);
            if (outfit == null)
            {
                return Result<OutfitDTO>.Failure("Outfit not found");
            }
            var outfitDto = mapper.Map<OutfitDTO>(outfit);
            return Result<OutfitDTO>.Success(outfitDto);
        }
    }

}
