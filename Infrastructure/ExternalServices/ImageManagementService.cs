using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using Domain.Common;
using Google.Cloud.Storage.V1;
using Application.Services;

namespace Infrastructure.Services
{
    public class ImageManagementService : IImageManagementService
    {
        private readonly HttpClient _http;
        private readonly string _endpointUrl;
        private const string ProjectId = "rewear-458908";
        private const string Location = "us-central1"; 
        private const string ModelId = "gemini-2.0-flash-001";
        private const string BucketName = "rewear";

        public ImageManagementService(HttpClient? http = null)
        {
            _http = http ?? new HttpClient();
            _endpointUrl =
              $"https://{Location}-aiplatform.googleapis.com/v1/projects/{ProjectId}/locations/{Location}/publishers/google/models/{ModelId}:generateContent";

            Console.WriteLine($"Endpoint URL: {_endpointUrl}");
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var credential = await GoogleCredential.GetApplicationDefaultAsync().ConfigureAwait(false);
            if (credential.IsCreateScopedRequired)
                credential = credential.CreateScoped("https://www.googleapis.com/auth/cloud-platform");

            var tokenAccess = (ITokenAccess)credential;
            return await tokenAccess.GetAccessTokenForRequestAsync("https://www.googleapis.com/auth/cloud-platform").ConfigureAwait(false);
        }
        private static string ExtractJson(string input)
        {
            input = input.Trim();

            // Elimina eventualele delimitatoare ``` sau explicatii
            if (input.StartsWith("```"))
            {
                Console.WriteLine("Removing code block delimiters");
                var start = input.IndexOf('{');
                var end = input.LastIndexOf('}');
                if (start >= 0 && end >= 0 && end > start)
                {
                    return input.Substring(start, end - start + 1);
                }
            }

            if (input.StartsWith("{") && input.EndsWith("}"))
                return input;

            var firstBrace = input.IndexOf('{');
            var lastBrace = input.LastIndexOf('}');
            if (firstBrace >= 0 && lastBrace > firstBrace)
                return input.Substring(firstBrace, lastBrace - firstBrace + 1);

            return input; // fallback
        }

        // Main method that handles image analysis and deserialization
        public async Task<Result<T>> AnalyzeImageAsync<T>(string prompt, params byte[][] images)
        {

            int retries = 3;
            while (retries-- > 0)
            {
                try
                {
                    // Get credentials and access token
                    string accessToken = await GetAccessTokenAsync();
                    // Prepare the request payload
                    var parts = new List<object> { new { text = prompt } };
                    parts.AddRange(images.Select(img => new { inlineData = new { mimeType = "image/jpeg", data = Convert.ToBase64String(img) } }));

                    var payload = new
                    {
                        contents = new[]
                        {
                            new
                            {
                                role = "user",
                                parts = parts.ToArray()
                            }
                        },
                        generationConfig = new
                        {
                            temperature = 0.2,
                            topP = 0.8,
                            maxOutputTokens = 8192
                        },
                        safetySettings = new[]
                        {
                            new { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_NONE" },
                            new { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_NONE" },
                            new { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "BLOCK_NONE" },
                            new { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_NONE" }
                        }
                    };

                    var json = JsonSerializer.Serialize(payload);
                    using var request = new HttpRequestMessage(HttpMethod.Post, _endpointUrl)
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    };
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    // Send the request and receive the response
                    using var response = await _http.SendAsync(request).ConfigureAwait(false);
                    var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                        return Result<T>.Failure($"HTTP request failed: {response.StatusCode}, {responseJson}");

                    // Clean and validate the JSON response
                    Console.WriteLine($"Response: {responseJson}\n\n");

                    var cleanText = ExtractJson(responseJson);
                    Console.WriteLine($"Cleaned Text:{cleanText}\n\n");
                    using var document = JsonDocument.Parse(cleanText); // Validate JSON structure
                    Console.WriteLine($"Document: {document.RootElement}\n\n");
                    var text = document.RootElement
                                    .GetProperty("candidates")[0]
                                    .GetProperty("content")
                                    .GetProperty("parts")[0]
                                    .GetProperty("text")
                                    .GetString() ?? "";
                    Console.WriteLine($"Response: {text}");


                    var cleanedJson = ExtractJson(text);

                    var result = JsonSerializer.Deserialize<T>(cleanedJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // If the deserialization fails, return an error
                    return result is null ? Result<T>.Failure("Model returned null or invalid JSON.") : Result<T>.Success(result);
                }
                catch (JsonException jsonEx)
                {
                    // If JSON is invalid, log the error and retry
                    return Result<T>.Failure($"Invalid JSON format: {jsonEx.Message} ");
                }
                catch (Exception ex)
                {
                    // If any other exception occurs, return the error message
                    return Result<T>.Failure($"Unexpected error: {ex.Message}");
                }
            }

            return Result<T>.Failure("Max retries reached. Could not get a valid result.");
        }

        public async Task<Result<List<T>>> AnalyzeImageListAsync<T>(string prompt, params byte[][] images)
        {
            int retries = 3;
            while (retries-- > 0)
            {
                try
                {
                    string accessToken = await GetAccessTokenAsync();
                    var parts = new List<object> { new { text = prompt } };
                    parts.AddRange(images.Select(img => new { inlineData = new { mimeType = "image/jpeg", data = Convert.ToBase64String(img) } }));

                    var payload = new
                    {
                        contents = new[]
                        {
                    new
                    {
                        role = "user",
                        parts = parts.ToArray()
                    }
                },
                        generationConfig = new
                        {
                            temperature = 0.2,
                            topP = 0.8,
                            maxOutputTokens = 8192
                        },
                        safetySettings = new[]
                        {
                    new { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_NONE" },
                    new { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_NONE" },
                    new { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "BLOCK_NONE" },
                    new { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_NONE" }
                }
                    };

                    var json = JsonSerializer.Serialize(payload);
                    using var request = new HttpRequestMessage(HttpMethod.Post, _endpointUrl)
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    };
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    using var response = await _http.SendAsync(request).ConfigureAwait(false);
                    var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                        return Result<List<T>>.Failure($"HTTP request failed: {response.StatusCode}, {responseJson}");

                    var cleanText = ExtractJson(responseJson);
                    using var document = JsonDocument.Parse(cleanText);
                    var text = document.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString() ?? "";

                    var cleanedJson = ExtractJson(text);
                    string jsonToDeserialize = cleanedJson.Trim();
                    if (jsonToDeserialize.StartsWith("{") && !jsonToDeserialize.StartsWith("["))
                    {
                        // If it looks like a sequence of objects, wrap in []
                        jsonToDeserialize = "[" + jsonToDeserialize + "]";
                    }
                    Console.WriteLine($"Final JSON to deserialize: {jsonToDeserialize}");

                    List<T>? result = null;
                    try
                    {
                        result = JsonSerializer.Deserialize<List<T>>(jsonToDeserialize, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true 
                        });
                    }
                    catch (JsonException jsonEx)
                    {
                        return Result<List<T>>.Failure($"Invalid JSON format: {jsonEx.Message}");
                    }

                    return result is null
                        ? Result<List<T>>.Failure("Model returned null or invalid JSON.")
                        : Result<List<T>>.Success(result);
                }
                catch (JsonException jsonEx)
                {
                    return Result<List<T>>.Failure($"Invalid JSON format: {jsonEx.Message} ");
                }
                catch (Exception ex)
                {
                    return Result<List<T>>.Failure($"Unexpected error: {ex.Message}");
                }
            }

            return Result<List<T>>.Failure("Max retries reached. Could not get a valid result.");
        }

        public async Task<Result<string>> UploadImageAsync(
            byte[] image,
            string userId,
            string entityId,
            string entityType,
            string imageType,
            string extension = "jpg")  // implicit .jpg
        {
            try
            {
                // Generare ID unic (Guid)
                string uniqueId = Guid.NewGuid().ToString();

                using var stream = new MemoryStream(image);

                var storageClient = await StorageClient.CreateAsync();

                // Exemplu de fileName: clothing-items/USERID/ENTITYID_front_uniqueid.jpg
                string fileName = $"{entityType}/{userId}/{entityId}_{imageType}_{uniqueId}.{extension}";

                var obj = new Google.Apis.Storage.v1.Data.Object
                {
                    Bucket = BucketName,
                    Name = fileName,
                    ContentType = $"image/{extension}" // jpeg sau png
                };

                var uploaded = await storageClient.UploadObjectAsync(obj, stream);

                // Returnăm gs://bucket_name/object_name
                string gsUri = $"gs://{uploaded.Bucket}/{uploaded.Name}";
                return Result<string>.Success(gsUri);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Image upload failed: {ex.Message}");
            }
        }



        public async Task<bool> ImageExistsAsync(string fileName)
        {
            try
            {
                var client = await StorageClient.CreateAsync();
                await client.GetObjectAsync(BucketName, fileName);
                return true;
            }
            catch (Google.GoogleApiException e) when (e.Error.Code == 404)
            {
                return false;
            }
        }

        public async Task<Result<bool>> DeleteImageAsync(string fileName)
        {
            try
            {
                var client = await StorageClient.CreateAsync();
                await client.DeleteObjectAsync(BucketName, fileName);
                return Result<bool>.Success(true);
            }
            catch (Google.GoogleApiException ex) when (ex.Error.Code == 404)
            {
                return Result<bool>.Failure("The image was not found in the bucket.");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Failed to delete the image: {ex.Message}");
            }
        }


        public async Task<List<string>> ListImagesAsync()
        {
            var client = await StorageClient.CreateAsync();
            var images = new List<string>();

            await foreach (var obj in client.ListObjectsAsync(BucketName, ""))
            {
                images.Add($"gs://{BucketName}/{obj.Name}");
            }

            return images;
        }

        public async Task<List<string>> ListImageBase64Async()
        {
            var client = await StorageClient.CreateAsync();
            var imageList = new List<string>();

            await foreach (var obj in client.ListObjectsAsync(BucketName, ""))
            {
                using var memoryStream = new MemoryStream();
                await client.DownloadObjectAsync(obj, memoryStream);
                var bytes = memoryStream.ToArray();
                var base64 = Convert.ToBase64String(bytes);
                var mimeType = obj.ContentType ?? "image/jpeg";
                imageList.Add($"data:{mimeType};base64,{base64}");
            }

            return imageList;
        }

    }
}
