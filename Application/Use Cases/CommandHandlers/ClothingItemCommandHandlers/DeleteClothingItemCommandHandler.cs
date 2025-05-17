using Application.Use_Cases.Commands.ClothingItemCommand;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.ClothingItemCommandHandlers
{
    public class DeleteClothingItemCommandHandler : IRequestHandler<DeleteClothingItemCommand, Result<Unit>>
    {
        private readonly IClothingItemRepository clothingItemRepository;
        public DeleteClothingItemCommandHandler(IClothingItemRepository clothingItemRepository)
        {
            this.clothingItemRepository = clothingItemRepository;
        }
        public async Task<Result<Unit>> Handle(DeleteClothingItemCommand request, CancellationToken cancellationToken)
        {
            var clothingItem = await clothingItemRepository.GetByIdAsync(request.Id);
            if (clothingItem == null)
            {
                return Result<Unit>.Failure("Clothing item not found");
            }
            await clothingItemRepository.DeleteAsync(clothingItem.Id);
            return Result<Unit>.Success(Unit.Value);
        }
    }

}
