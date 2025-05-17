using Application.Services;
using Google.Apis.Auth.OAuth2;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly HttpClient _http;
        private readonly string _endpointUrl;
        private readonly string _projectId = "rewear-458908"; // Înlocuiește cu ID-ul proiectului tău GCP
        private readonly string _locationId = "us-central1"; // Înlocuiește cu regiunea endpoint-ului tău (verifică documentația)
        private readonly string _modelId = "text-embedding-005";

        public EmbeddingService(HttpClient? http = null)
        {
            _http = http ?? new HttpClient();
            _endpointUrl = $"https://{_locationId}-aiplatform.googleapis.com/v1/projects/{_projectId}/locations/{_locationId}/publishers/google/models/{_modelId}:predict";
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var credential = await GoogleCredential.GetApplicationDefaultAsync().ConfigureAwait(false);
            if (credential.IsCreateScopedRequired)
                credential = credential.CreateScoped("https://www.googleapis.com/auth/cloud-platform");

            var tokenAccess = (ITokenAccess)credential;
            return await tokenAccess.GetAccessTokenForRequestAsync("https://www.googleapis.com/auth/cloud-platform").ConfigureAwait(false);
        }

        public async Task<float[]> GetEmbeddingAsync(string text)
        {
            var accessToken = await GetAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
            {
                return null;
            }

            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var requestBody = new
            {
                instances = new[]
                {
                    new
                    {
                        task_type = "RETRIEVAL_DOCUMENT",
                        title = "Rewear", 
                        content = text
                    }
                }
            };

            var jsonRequest = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            try
            {
                var response = await _http.PostAsync(_endpointUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                    {
                        if (doc.RootElement.TryGetProperty("predictions", out var predictions) &&
                            predictions.GetArrayLength() > 0 &&
                            predictions[0].TryGetProperty("embeddings", out var embeddings) &&
                            embeddings.TryGetProperty("values", out var values) &&
                            values.ValueKind == JsonValueKind.Array)
                        {
                            return values.EnumerateArray().Select(v => v.GetSingle()).ToArray();
                        }
                        else if (doc.RootElement.TryGetProperty("predictions", out predictions) &&
                                 predictions.GetArrayLength() > 0 &&
                                 predictions[0].TryGetProperty("embedding", out var embeddingObject) &&
                                 embeddingObject.TryGetProperty("values", out values) &&
                                 values.ValueKind == JsonValueKind.Array)
                        {
                            // Alternativ format de răspuns
                            return values.EnumerateArray().Select(v => v.GetSingle()).ToArray();
                        }
                        else
                        {
                            Console.WriteLine($"Format de răspuns neașteptat: {jsonResponse}");
                            return null;
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Eroare la cerere: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Eroare de rețea: {ex.Message}");
                return null;
            }
        }

        public float ComputeCosineSimilarity(float[] vector1, float[] vector2)
        {
            if (vector1 == null || vector2 == null || vector1.Length != vector2.Length)
            {
                return 0f;
            }

            float dotProduct = 0;
            float magnitude1 = 0;
            float magnitude2 = 0;

            for (int i = 0; i < vector1.Length; i++)
            {
                dotProduct += vector1[i] * vector2[i];
                magnitude1 += vector1[i] * vector1[i];
                magnitude2 += vector2[i] * vector2[i];
            }

            magnitude1 = (float)Math.Sqrt(magnitude1);
            magnitude2 = (float)Math.Sqrt(magnitude2);

            if (magnitude1 == 0 || magnitude2 == 0)
            {
                return 0f;
            }

            return dotProduct / (magnitude1 * magnitude2);
        }
    }
}