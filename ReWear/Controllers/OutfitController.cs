using Application.Models;
using Application.Services;
using Application.Use_Cases.Commands.ClothingItemCommand;
using Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ReWear.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OutfitController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IOutfitService outfitService;
        private readonly IOutfitRepository outfitRepository;

        public OutfitController(IMediator mediator, IOutfitService outfitService, IOutfitRepository outfitRepository)
        {
            this.mediator = mediator;
            this.outfitService = outfitService;
            this.outfitRepository = outfitRepository;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> Analyze([FromForm] AnalyzeClothingItemCommand request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Convert files to byte[]
            byte[] frontBytes, backBytes;
            await using (var ms1 = new MemoryStream())
            {
                await request.ImageFront.CopyToAsync(ms1);
                frontBytes = ms1.ToArray();
            }
            await using (var ms2 = new MemoryStream())
            {
                await request.ImageBack.CopyToAsync(ms2);
                backBytes = ms2.ToArray();
            }
            // Call the image analysis service
            var result = await outfitService.AnalyzeClothingItemsAsync(frontBytes);
            if (!result.IsSuccess)
            {
                // Return the error message with a 502 status code
                return StatusCode(502, result.ErrorMessage);
            }
            // Return the successful result
            return Ok(result);
        }
    }
}
