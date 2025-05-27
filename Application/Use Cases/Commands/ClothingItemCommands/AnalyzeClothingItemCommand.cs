using Application.Models;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.Use_Cases.Commands.ClothingItemCommand
{
    public class AnalyzeClothingItemCommand : IRequest<Result<ImageAnalysisResult>>
    {
        [Required] public required byte[] ImageFront { get; set; } 
         public byte[]? ImageBack { get; set; } 
    }
}
