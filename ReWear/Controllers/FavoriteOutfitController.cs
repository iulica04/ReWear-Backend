using Application.Use_Cases.Commands.FavoriteOutfitCommands;
using Application.Use_Cases.Queries.FavoriteOutfitQueries;
using Application.Use_Cases.Queries.OutfitQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ReWear.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoriteOutfitController : ControllerBase
    {
        private readonly IMediator mediator;

        public FavoriteOutfitController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Guid>> CreateFavoriteOutfit([FromBody] CreateFavoriteOutfitCommand command)
        {
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);
            return CreatedAtAction(nameof(GetFavoriteOutfitById), new { id = result.Data }, result.Data);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult> GetFavoriteOutfitById(Guid id)
        {
            var result = await mediator.Send(new GetByIdQuery { Id = id });
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpGet("outfits/user/{userId}")]
        [Authorize]
        public async Task<ActionResult> GetFavoriteOutfitsByUserId(Guid userId, [FromQuery] int page, [FromQuery] int pageSize)
        { 
            var result = await mediator.Send(new GetFavoriteOutfitsByUserIdQuery { UserId = userId, Page = page, PageSize = pageSize});
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<ActionResult> GetAllFavoriteOutfitsByUserId(Guid userId)
        {
            var result = await mediator.Send(new GetUserFavoriteOutfitRecordsQuery { UserId = userId });
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpDelete("{userId}/{outfitId}")]
        [Authorize]
        public async Task<IActionResult> DeleteFavoriteOutfit(Guid userId, Guid outfitId)
        {
            var result = await mediator.Send(new DeleteFavoriteOutfitCommand {UserId = userId, OutfitId = outfitId});
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);
            return NoContent();
        }
    }
}
