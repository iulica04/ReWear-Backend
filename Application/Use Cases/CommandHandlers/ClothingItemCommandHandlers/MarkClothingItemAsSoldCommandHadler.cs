using Application.Use_Cases.Commands.ClothingItemCommands;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.ClothingItemCommandHandlers
{
    public class MarkClothingItemAsSoldCommandHadler : IRequestHandler<MarkClothingItemAsSoldCommand, Result<Unit>>
    {
        private readonly IClothingItemRepository repository;

        public MarkClothingItemAsSoldCommandHadler(IClothingItemRepository repository)
        {
            this.repository = repository;
        }
        public async Task<Result<Unit>> Handle(MarkClothingItemAsSoldCommand request, CancellationToken cancellationToken)
        {
            var clothingItem = await repository.GetByIdAsync(request.Id);
            if (clothingItem == null)
            {
                return Result<Unit>.Failure("Clothing item not found");
            }
            clothingItem.IsSold = true;
            await repository.UpdateAsync(clothingItem);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
