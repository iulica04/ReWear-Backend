using Application.Models;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ReWear.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OLXController : ControllerBase
    {
        private readonly IOLXServices _olxService;

        public OLXController(IOLXServices olxService)
        {
            _olxService = olxService;
        }

        // POST api/olx/postanunt
        [HttpPost("postanunt")]
        public async Task<IActionResult> PostAnuntAsync([FromBody] AnuntModel anunt)
        {
            if (anunt == null)
            {
                return BadRequest("Anunțul nu poate fi null.");
            }

            var success = await _olxService.PostAnuntAsync(anunt);

            if (success)
            {
                return Ok("Anunțul a fost postat cu succes!");
            }

            return StatusCode(500, "A apărut o eroare la postarea anunțului.");
        }
    }
}
