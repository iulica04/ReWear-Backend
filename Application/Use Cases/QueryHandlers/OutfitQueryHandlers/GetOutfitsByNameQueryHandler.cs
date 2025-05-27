// Application/Use Cases/QueryHandlers/OutfitQueryHandlers/GetOutfitsByNameQueryHandler.cs
using MediatR;
using Domain.Repositories;
using Application.DTOs;
using Domain.Common;
using AutoMapper;
using Application.Use_Cases.Queries.OutfitQueries;

public class GetOutfitsByNameQueryHandler : IRequestHandler<GetOutfitsByNameQuery, Result<List<OutfitDTO>>>
{
    private readonly IOutfitRepository outfitRepository;
    private readonly IMapper mapper;

    public GetOutfitsByNameQueryHandler(IOutfitRepository outfitRepository, IMapper mapper)
    {
        this.outfitRepository = outfitRepository;
        this.mapper = mapper;
    }

    public async Task<Result<List<OutfitDTO>>> Handle(GetOutfitsByNameQuery request, CancellationToken cancellationToken)
    {
        var allOutfits = await outfitRepository.GetAllAsync();
        var filtered = allOutfits
            .Where(o => o.UserId == request.UserId &&
                        !string.IsNullOrEmpty(o.Name) &&
                        o.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var dtos = mapper.Map<List<OutfitDTO>>(filtered);
        return Result<List<OutfitDTO>>.Success(dtos);
    }
}