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
        public async Task<IActionResult> Analyze([FromBody] AnalyzeClothingItemCommand request)
        {
          
            var result = await mediator.Send(request);

            if (!result.IsSuccess)
                return StatusCode(502, result.ErrorMessage);

            return Ok(result.Data);
        }



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
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }
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
        public async Task<ActionResult<PagedResult<ClothingItemDTO>>> GetPaginatedClothingItems(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] List<string>? printTypes,
            [FromQuery] List<string>? brands,
            [FromQuery] List<string>? colors,
            [FromQuery] List<string>? categories,
            [FromQuery] List<string>? tags,
            [FromQuery] Guid userId)
        {
            Expression<Func<ClothingItem, bool>> filter = item =>
            (printTypes == null || printTypes.Count == 0 || printTypes.Contains(item.PrintType)) &&
            (brands == null || brands.Count == 0 || brands.Contains(item.Brand)) &&
            (colors == null || colors.Count == 0 || colors.Contains(item.Color)) &&
            (categories == null || categories.Count == 0 || categories.Contains(item.Category)) &&
            (tags == null || tags.Count == 0 || item.Tags.Any(t => tags.Contains(t.Tag))) &&
            (item.UserId == userId);


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

        [HttpGet("get-by-name")]
        public async Task<IActionResult> GetClothingItemsByName([FromQuery] GetClothingItemsByNameQuery query)
        {
            var result = await mediator.Send(query);

            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);

            return Ok(result.Data);
        }

    }
}
