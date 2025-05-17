using Application.Use_Cases.Commands.ClothingItemCommand;
using AutoMapper;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.ClothingItemCommandHandlers
{
    public class UpdateClothingItemCommandHandler : IRequestHandler<UpdateClothingItemCommand>
    {
        private readonly IClothingItemRepository clothingItemRepository;
        private readonly IMapper mapper;
        public UpdateClothingItemCommandHandler(IClothingItemRepository clothingItemRepository, IMapper mapper)
        {
            this.clothingItemRepository = clothingItemRepository;
            this.mapper = mapper;
        }

        public async Task Handle(UpdateClothingItemCommand request, CancellationToken cancellationToken)
        {
            var clothingItem = mapper.Map<Domain.Entities.ClothingItem>(request);
            await clothingItemRepository.UpdateAsync(clothingItem);
        }
    }
}
