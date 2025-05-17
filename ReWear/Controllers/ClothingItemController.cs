using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Domain.Repositories;
using Application.Use_Cases.Commands.ClothingItemCommand;
using MediatR;
using Application.Use_Cases.Queries.ClothingItemQueries;
using Application.Utils;
using System.Linq.Expressions;
using Domain.Entities;
using Application.Use_Cases.Queries;
using Application.Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClothingItemController : ControllerBase
    {
        private readonly IClothingItemService clothingItemService;
        private readonly IMediator mediator;

        public ClothingItemController(IClothingItemService clothingItemService, IMediator mediator)
        {
            this.clothingItemService = clothingItemService;
            this.mediator = mediator;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> Analyze([FromForm] AnalyzeClothingItemCommand request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            byte[] frontBytes;
            byte[]? backBytes = null;

            await using (var ms1 = new MemoryStream())
            {
                await request.ImageFront.CopyToAsync(ms1);
                frontBytes = ms1.ToArray();
            }
            if (request.ImageBack != null)
            {
                await using (var ms2 = new MemoryStream())
                {
                    await request.ImageBack.CopyToAsync(ms2);
                    backBytes = ms2.ToArray();
                }
            }

            var result = await clothingItemService.AnalyzeClothingItemsAsync(frontBytes, backBytes);

            if (!result.IsSuccess)
                return StatusCode(502, result.ErrorMessage);

            return Ok(result.Data);
        }

        //[HttpPost("upload")]
        //public async Task<IActionResult> UploadImage(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("No file uploaded.");

        //    using var memoryStream = new MemoryStream();
        //    await file.CopyToAsync(memoryStream);
        //    var imageBytes = memoryStream.ToArray();

        //    var result = await clothingItemService.UploadImageAsync(imageBytes, file.FileName);

        //    if (!result.IsSuccess)
        //        return StatusCode(500, result.ErrorMessage);

        //    return Ok(new { path = result.Data });
        //}

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateClothingItem([FromBody] CreateClothingItemCommand command)
        {
            var result = await mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);
            return CreatedAtAction(nameof(GetClothingItemById), new { Id = result.Data }, result.Data);
        }


        [HttpDelete("delete-image-from-bucket")]
        public async Task<IActionResult> DeleteImageFromBucket(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return BadRequest("No image path provided.");
            var result = await clothingItemService.DeleteImageAsync(imagePath);
            if (!result.IsSuccess)
                return StatusCode(500, result.ErrorMessage);
            return Ok(new { message = "Image deleted successfully." });
        }

        [HttpGet("get-all-images-base64")]
        public async Task<IActionResult> GetAllImagesAsBase64()
        {
            var images = await clothingItemService.ListImageBase64Async();
            return Ok(images);
        }

        [HttpGet("list-images-from-cloud")]
        public async Task<IActionResult> GetAllImages()
        {
            var images = await clothingItemService.ListImagesAsync();
            return Ok(images);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClothingItemById(Guid id)
        {
            var result = await mediator.Send(new GetClothingItemByIdQuery { Id = id });
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllClothingItems()
        {
            var result = await mediator.Send(new GetAllClothingItemsQuery());
            return Ok(result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClothingItem(Guid id, [FromBody] UpdateClothingItemCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Clothing item ID mismatch");
            }
            await mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClothingItem(Guid id)
        {
            var result = await mediator.Send(new DeleteClothingItemCommand(id));
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpGet("paginated")]
        public async Task<ActionResult<PagedResult<ClothingItemDTO>>> GetPaginatedClothingItems([FromQuery] int page, [FromQuery] string? printType,
            [FromQuery] int pageSize, [FromQuery] string? brand, [FromQuery] string? color, [FromQuery] string? category, string? tag)
        {
            Expression<Func<ClothingItem, bool>> filter = item =>
                (string.IsNullOrEmpty(printType) || item.PrintType == printType) &&
                (string.IsNullOrEmpty(brand) || item.Brand == brand) &&
                (string.IsNullOrEmpty(color) || item.Color == color) &&
                (string.IsNullOrEmpty(category) || item.Category == category)
                && (string.IsNullOrEmpty(tag) || item.Tags.Any(t => t.Tag == tag));

            var query = new GetFilteredQuery<ClothingItem, ClothingItemDTO>
            {
                Page = page,
                PageSize = pageSize,
                Filter = filter
            };

            var result = await mediator.Send(query);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return NotFound(result.ErrorMessage);
        }
   
    }
}
