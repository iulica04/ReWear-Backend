using Application.Models;
using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.ClothingItemCommands
{
    public class EstimateCarbonFootprintCommand : IRequest<Result<EstimateCarbonFootprintResult>>
    {
        public Guid UserId { get; set; }
    }
}
