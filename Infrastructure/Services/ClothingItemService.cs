using Application.Models;
using Application.Services;
using Domain.Common;


namespace Infrastructure.Services
{
    public class ClothingItemService : IClothingItemService
    {
        private readonly IImageManagementService imageManagementServices;

        public ClothingItemService(IImageManagementService imageManagementServices)
        {
            this.imageManagementServices = imageManagementServices;
        }

        public async Task<Result<ImageAnalysisResult>> AnalyzeClothingItemsAsync(byte[] imageFront, byte[]? imageBack)
        {
            // Call ImageAnalyzerServices to get the raw analysis result
            string prompt =
                "Generate a single-line, compact, valid JSON object with the following exact fields:\n" +
                "- Name (string)\n" +
                "- Category (string)\n" +
                "- Tags (array of strings)\n" +
                "- Color (simple color name, string)\n" +
                "- Brand (string)\n" +
                "- Material (string)\n" +
                "- PrintType (string or null)\n" +
                "- PrintDescription (string or null)\n" +
                "- Description (string)\n\n" +
                "All string values must be enclosed in double quotes. Tags must be a JSON array (e.g., [\"Casual\", \"Summer\"]). Do not include markdown, explanations, or line breaks. Return **only** the raw JSON.\n\n" +
                "Example:\n" +
                "{\"Name\":\"T-Shirt\",\"Category\":\"Topwear\",\"Tags\":[\"Casual\",\"Summer\"],\"Color\":\"Red\",\"Brand\":\"Nike\",\"Material\":\"Cotton\",\"PrintType\":\"Logo\",\"PrintDescription\":\"Nike swoosh on front\",\"Description\":\"A red cotton T-shirt.\"}";

            if(imageBack == null)
            {
                var analysisResult = await imageManagementServices.AnalyzeImageAsync<ImageAnalysisResult>(prompt, imageFront);
                if (!analysisResult.IsSuccess)
                {
                    // Return failure if the analysis failed
                    return Result<ImageAnalysisResult>.Failure(analysisResult.ErrorMessage);
                }

                return Result<ImageAnalysisResult>.Success(analysisResult.Data);

            }
            else
            {
                var analysisResult = await imageManagementServices.AnalyzeImageAsync<ImageAnalysisResult>(prompt, imageFront, imageBack);
                if (!analysisResult.IsSuccess)
                {
                    // Return failure if the analysis failed
                    return Result<ImageAnalysisResult>.Failure(analysisResult.ErrorMessage);
                }

                return Result<ImageAnalysisResult>.Success(analysisResult.Data);

            }
        }

        public Task<Result<bool>> DeleteImageAsync(string fileName)
        {
            return imageManagementServices.DeleteImageAsync(fileName);
        }

        public Task<List<string>> ListImageBase64Async()
        {
            return imageManagementServices.ListImageBase64Async();
        }

        public Task<List<string>> ListImagesAsync()
        {
            return imageManagementServices.ListImagesAsync();
        }
 
        public Task<Result<string>> UploadImageAsync(byte[] image, string userId, string entityId, string entityType, string imageType, string extension = "jpg")
        {
            return imageManagementServices.UploadImageAsync(image, userId, entityId, entityType, imageType, extension);
        }
    }
}