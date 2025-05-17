using Application.DTOs;
using Application.Models;
using Application.Services;
using Domain.Common;

namespace Infrastructure.Services
{
    public class OutfitService : IOutfitService
    {
        private readonly IImageManagementService imageManagementService;

        public OutfitService(IImageManagementService imageManagementService)
        {
            this.imageManagementService = imageManagementService;
        }

        public async Task<Result<List<OutfitAnalysisResult>>> AnalyzeClothingItemsAsync(byte[] image)
        {
            string prompt =
                "You are a fashion analysis AI. Analyze the image(s) and return a compact JSON array, each item representing a visible clothing item without accesories. " +
                "Each clothing item must include:\n" +
                "- Name (string)\n" +
                "- Category (string)\n" +
                "- Tags (array of strings)\n" +
                "- Color (simple color name, string)\n" +
                "- Brand (string or \"unknown\")\n" +
                "- Material (string)\n" +
                "- PrintType (string or null)\n" +
                "- PrintDescription (string or null)\n" +
                "- Description (string)\n\n" +
                "Tags must be a JSON array (e.g., [\"casual\", \"summer\"]). Return ONLY the raw JSON array. No markdown, no explanation.\n\n" +
                "Return ONLY a JSON array (enclosed in square brackets), even if there is only one item. Do not return multiple objects separated by commas without brackets." +
                "Example:\n" +
                "[{\"Name\":\"White T-shirt\",\"Category\":\"shirt\",\"Tags\":[\"casual\"],\"Color\":\"white\",\"Brand\":\"unknown\",\"Material\":\"cotton\",\"PrintType\":\"solid\",\"PrintDescription\":\"plain white\",\"Description\":\"Simple white cotton T-shirt.\"}, " +
                "{\"Name\":\"Blue jeans\",\"Category\":\"pants\",\"Tags\":[\"casual\"],\"Color\":\"blue\",\"Brand\":\"Levi's\",\"Material\":\"denim\",\"PrintType\":\"solid\",\"PrintDescription\":\"classic blue wash\",\"Description\":\"Straight blue denim jeans.\"}]";

            var analysisResult = await imageManagementService.AnalyzeImageListAsync<OutfitAnalysisResult>(prompt, image);
            if (!analysisResult.IsSuccess)
                return Result<List<OutfitAnalysisResult>>.Failure(analysisResult.ErrorMessage);
            return Result<List<OutfitAnalysisResult>>.Success(analysisResult.Data);

        }

        public Task<Result<bool>> DeleteImageAsync(string fileName)
        {
            return imageManagementService.DeleteImageAsync(fileName);
        }

        public Task<List<string>> ListImagesAsync()
        {
            return imageManagementService.ListImagesAsync();
        }

        public Task<Result<string>> UploadImageAsync(byte[] image, string userId, string entityId, string entityType, string imageType, string extension = "jpg")
        {
            return imageManagementService.UploadImageAsync(image, userId, entityId, entityType, imageType, extension);
        }
    }
}
