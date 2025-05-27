using Application.Use_Cases.Commands.FavoriteOutfitCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.FavoriteOutfitCommandHandlers
{
    public class CreateFavoriteOutfitCommandHandler : IRequestHandler<CreateFavoriteOutfitCommand, Result<Guid>>
    {

        private readonly IMapper mapper;
        private readonly IFavoriteOutfitRepository repository;

        public CreateFavoriteOutfitCommandHandler(IMapper mapper, IFavoriteOutfitRepository repository)
        {
            this.mapper = mapper;
            this.repository = repository;
        }
        public async Task<Result<Guid>> Handle(CreateFavoriteOutfitCommand request, CancellationToken cancellationToken)
        {

            var existingFavorite = await repository.GetByUserAndOutfitAsync(request.UserId, request.OutfitId);
            if (existingFavorite != null)
            {
                return Result<Guid>.Failure("Outfit already added to favorites.");
            }

            var favoriteOutfit = new FavoriteOutfit
            {
                UserId = request.UserId,
                OutfitId = request.OutfitId
            };
            var result = await repository.AddAsync(favoriteOutfit);
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }
            else
            {
                return Result<Guid>.Failure("Failed to create favorite outfit");
            }
        }
    }
}
