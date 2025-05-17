using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ReWear.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmbeddingTestController : ControllerBase
    {
        private readonly IEmbeddingService _embeddingService;

        public EmbeddingTestController(IEmbeddingService embeddingService)
        {
            _embeddingService = embeddingService;
        }

        [HttpPost("get-embedding")]
        public async Task<IActionResult> GetEmbedding([FromBody] EmbeddingRequest request)
        {
            if (string.IsNullOrEmpty(request?.Text))
            {
                return BadRequest("Textul nu poate fi null sau gol.");
            }

            var embedding = await _embeddingService.GetEmbeddingAsync(request.Text);

            if (embedding != null && embedding.Length > 0)
            {
                return Ok(new { Embedding = embedding });
            }
            else
            {
                return StatusCode(500, "Nu s-a putut obține embedding-ul.");
            }
        }

        [HttpPost("compute-similarity")]
        public IActionResult ComputeSimilarity([FromBody] SimilarityRequest request)
        {
            if (request?.Vector1 == null || request.Vector1.Length == 0 || request.Vector2 == null || request.Vector2.Length == 0)
            {
                return BadRequest("Vectorii nu pot fi null sau goi.");
            }

            if (request.Vector1.Length != request.Vector2.Length)
            {
                return BadRequest("Vectorii trebuie să aibă aceeași dimensiune.");
            }

            var similarity = _embeddingService.ComputeCosineSimilarity(request.Vector1, request.Vector2);
            return Ok(new { Similarity = similarity });
        }
    }


    public class EmbeddingRequest
    {
        public string Text { get; set; }
    }

    public class SimilarityRequest
    {
        public float[] Vector1 { get; set; }
        public float[] Vector2 { get; set; }
    }
}
