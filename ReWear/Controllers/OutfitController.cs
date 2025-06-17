using Application.DTOs;
using Application.Models;
using Application.Services;
using Application.Use_Cases.Commands.ClothingItemCommand;
using Application.Use_Cases.Commands.OutfitCommands;
using Application.Use_Cases.Queries;
using Application.Use_Cases.Queries.OutfitQueries;
using Domain.Entities;
using Domain.Repositories;
using Google.Cloud.AIPlatform.V1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace ReWear.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OutfitController : ControllerBase
    {
        private readonly IMediator mediator;

        public OutfitController(IMediator mediator)
        {
            this.mediator = mediator;

        }

        [HttpPost("analyze-items")]
        public async Task<IActionResult> AnalyzeOutfitItems([FromBody] AnalyzeOutfitItemsCommand command)
        {
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpPost("match-items")]
        public async Task<IActionResult> MatchOutfitItems([FromBody] MatchOutfitItemsCommand command)
        {
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data); // Lista cu top 3 ClothingItemDTO
        }

        [HttpPost("analyze-outfit")]
        public async Task<IActionResult> AnalyzeOutfit([FromBody] AnalyzeOutfitCommand command)
        {
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpPost("create-outfit")]
        public async Task<ActionResult<Guid>> CreateOutfit([FromBody] CreateOutfitCommand command)
        {
            var result = await mediator.Send(command);
            if(!result.IsSuccess) 
                return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOutfitById(Guid id)
        {
            var result = await mediator.Send(new GetOutfitByIdQuery { Id = id });
            if (result.IsSuccess)
                return Ok(result.Data);
            return NotFound(result.ErrorMessage);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOutfits()
        {
            var result = await mediator.Send(new GetAllOutfitsQuery());
            return Ok(result.Data);
        }

        [HttpGet("paginated")]
        public async Task<IActionResult> GetPaginatedOutfits([FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] Guid? userId, [FromQuery] Guid? clothingItemId, [FromQuery] string? season,
            [FromQuery] DateTime createdAt)
        {
            Expression<Func<Outfit, bool>> filter = item =>
            (userId == null || item.UserId == userId) &&
            (clothingItemId == null || item.OutfitClothingItems.Any(oci => oci.ClothingItemId == clothingItemId)) &&
            (string.IsNullOrEmpty(season) || item.Season == season);

            var query = new GetFilteredQuery<Outfit, OutfitDTO>
            {
                Page = page,
                PageSize = pageSize,
                Filter = filter
            };

            var result = await mediator.Send(query);
            return Ok(result.Data);

        }

        [HttpGet("get-by-name")]
        public async Task<IActionResult> GetOutfitsByName([FromQuery] GetOutfitsByNameQuery query)
        {
           
            var result = await mediator.Send(query);

            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOutfit(Guid id)
        {
            var result = await mediator.Send(new DeleteOutfitCommand(id));
            if (result.IsSuccess)
                return NoContent();
            return NotFound(result.ErrorMessage);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOutfit(Guid id, [FromBody] UpdateOutfitCommand command)
        {
            if(id != command.Id)
            {
                return BadRequest("Outfit ID mismatch");
            }
            var result = await mediator.Send(command);
            if(!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }
            return NoContent();
        }

        [HttpGet("similar")]
        public async Task<IActionResult> GetSimilarOutfits([FromQuery] GetPaginatedSimilarOutfitsQuery query)
        {
            var result = await mediator.Send(query);
            if (result.IsSuccess)
                return Ok(result.Data);
            return NotFound(result.ErrorMessage);
        }

        [HttpPost("review-outfit")]
        public async Task<IActionResult> ReviewOutfit([FromBody] ReviewOutfitCommand command)
        {
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
        }
    }
}
