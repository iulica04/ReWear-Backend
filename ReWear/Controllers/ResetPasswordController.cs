using Application.Use_Cases.ForgotPassword;
using Application.Use_Cases.ForgotPassword.ResetPassword;
using Application.Use_Cases.ForgotPassword.SendPasswordResetCode;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ReWear.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResetPasswordController : ControllerBase
    {
        private readonly IMediator mediator;

        public ResetPasswordController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("send-reset-code")]
        public async Task<IActionResult> SendResetCode([FromBody] SendResetPasswordCodeCommand command)
        {
            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            else if (result.ErrorMessage == "Email not found")
            {
                return NotFound(result.ErrorMessage);
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeCommand command)
        {
            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.ErrorMessage);
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.ErrorMessage);
        }

    }
}
