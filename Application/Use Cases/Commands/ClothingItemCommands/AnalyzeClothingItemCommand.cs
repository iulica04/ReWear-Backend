using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.Use_Cases.Commands.ClothingItemCommand
{
    public class AnalyzeClothingItemCommand
    {
        [Required] public IFormFile ImageFront { get; set; } = default!;
         public IFormFile? ImageBack { get; set; } = default!;
    }
}
