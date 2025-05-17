namespace Application.Services
{
    public interface IEmbeddingService
    {
        Task<float[]> GetEmbeddingAsync(string text);
        float ComputeCosineSimilarity(float[] vector1, float[] vector2);
    }
}
