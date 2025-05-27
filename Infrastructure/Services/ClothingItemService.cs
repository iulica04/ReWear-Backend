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
                "The Description must be strictly technical and detailed. It should include observable structural and material aspects of the clothing item, such as:\n" +
                "- Color (e.g., blue, pink, navy blue, light grey, olive green)\n" +
                "- Fit and cut (e.g., slim fit, oversized, cropped)\n" +
                "- Closure mechanism (e.g., button, zipper, drawstring)\n" +
                "- Seam and stitching types (e.g., topstitch, double stitch)\n" +
                "- Edge finishes (e.g., ribbed hem, raw edge, cuffed sleeves)\n" +
                "- Sleeve, collar or neckline specifications\n" +
                "- Fabric texture or construction (e.g., knit, woven, fleece)\n" +
                "- Presence and type of pockets, labels, visible logos, etc.\n" +
                "The description must be factual and objective. Do not use expressive or marketing-style language.\n\n" +
                "All string values must be enclosed in double quotes. Tags must be a JSON array contains style labels (e.g., [\"Casual\", \"Minimalist\"]). Do not include markdown, explanations, or line breaks. Return **only** the raw JSON.\n\n" +
                "Example:\n" +
                "{\"Name\":\"T-Shirt\",\"Category\":\"Topwear\",\"Tags\":[\"Casual\",\"Minimalist\"],\"Color\":\"Red\",\"Brand\":\"Nike\",\"Material\":\"Cotton\",\"PrintType\":\"Logo\",\"PrintDescription\":\"Nike swoosh on front\",\"Description\":\"Red short-sleeved T-shirt with crew neck, made of lightweight cotton jersey. Ribbed neckline, straight stitched hem, and set-in sleeves. Front logo print centered on chest.\"}";

            if (imageBack == null)
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