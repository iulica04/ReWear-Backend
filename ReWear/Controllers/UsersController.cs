using Application.Use_Cases.Authentification;
using Application.Use_Cases.Commands;
using Application.Use_Cases.Commands.UserCommands;
using Application.Use_Cases.Queries.UserQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ReWear.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator mediator;

        public UsersController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateUser(CreateUserCommand command)
        {
            var result = await mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage); 

            return CreatedAtAction(nameof(GetUserById), new { Id = result.Data }, result.Data);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await mediator.Send(new GetUserByIdQuery { Id = id });
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await mediator.Send(new GetAllUsersQuery());
            return Ok(result.Data);
        }

        [HttpPut("id")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("User ID mismatch");
            }
            await mediator.Send(command);
            return NoContent();
        }

        [HttpHead("check-existence/{emailOrUsername}")]
        public async Task<IActionResult> CheckUserExistence(string emailOrUsername)
        {
            var result = await mediator.Send(new CheckUserExistenceQuery { EmailOrUsername = emailOrUsername });
            if (result.IsSuccess)
            {
                return Ok();
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await mediator.Send(new DeleteUserCommand(id));
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return Unauthorized(result.ErrorMessage);
        }

        [HttpPost("login-with-firebase")]
        public async Task<IActionResult> LoginFirebase(LoginCommand command)
        {
            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return Unauthorized(result.ErrorMessage);

        }
    }
}
