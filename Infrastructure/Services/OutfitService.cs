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

        public async Task<Result<List<OutfitItemsAnalysisResult>>> AnalyzeClothingItemsAsync(byte[] image)
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
                "Tags must be a JSON array (e.g., [\"casual\", \"summer\"]).\n\n" +
                "The Description field must be highly detailed and strictly technical. It should include all observable construction details, such as:\n" +
                "- Color (e.g., blue, pink, navy blue, light grey, olive green)\n" +
                "- Garment type and cut (e.g., regular fit, oversized, cropped, high-rise, wide-leg)\n" +
                "- Closure system (e.g., zipper, button, elastic waistband, drawstring)\n" +
                "- Stitching and seam details (e.g., double stitching, overlock seams, visible topstitch)\n" +
                "- Hem and edge finishes (e.g., raw hem, folded, cuffed, ribbed)\n" +
                "- Collar/neckline/sleeve specifications (e.g., V-neck, crew neck, raglan sleeve, short sleeve)\n" +
                "- Fabric texture or visible structure (e.g., ribbed knit, woven, fleece-lined)\n" +
                "- Presence of pockets, vents, slits, pleats, drawcords or reinforcement areas\n" +
                "- Placement of labels, logos, or patches if visible\n\n" +
                "The language must be technical and objective. Do not use fashion catalog or marketing style. Do not use expressive or subjective language. Keep it factual and neutral.\n\n" +
                "Return ONLY the raw JSON array. No markdown, no explanation.\n" +
                "Return ONLY a JSON array (enclosed in square brackets), even if there is only one item. Do not return multiple objects separated by commas without brackets.\n\n" +
                "Example:\n" +
                "[{\"Name\":\"White T-shirt\",\"Category\":\"shirt\",\"Tags\":[\"casual\",\"summer\"],\"Color\":\"white\",\"Brand\":\"unknown\",\"Material\":\"cotton\",\"PrintType\":\"solid\",\"PrintDescription\":\"plain white\",\"Description\":\"Short-sleeved T-shirt with crew neckline, made of lightweight cotton jersey. Features ribbed collar, straight hem with single stitching, and set-in sleeves. No visible logos or prints.\"}," +
                "{\"Name\":\"Blue jeans\",\"Category\":\"pants\",\"Tags\":[\"casual\",\"streetwear\"],\"Color\":\"blue\",\"Brand\":\"Levi's\",\"Material\":\"denim\",\"PrintType\":\"solid\",\"PrintDescription\":\"classic blue wash\",\"Description\":\"High-rise five-pocket jeans made of medium-weight denim with a stonewashed finish. Features metal button closure, zip fly, belt loops, and wide-leg cut. Double stitched side seams and folded stitched hems.\"}]";

            var analysisResult = await imageManagementService.AnalyzeImageListAsync<OutfitItemsAnalysisResult>(prompt, image);
            if (!analysisResult.IsSuccess)
                return Result<List<OutfitItemsAnalysisResult>>.Failure(analysisResult.ErrorMessage);
            return Result<List<OutfitItemsAnalysisResult>>.Success(analysisResult.Data);

        }

        public async Task<Result<OutfitAnalysisResult>> AnalyzeOutfitAsync(byte[] image)
        {
            var promp =
              "You are a fashion assistant AI. Analyze the outfit in the image and generate a single, compact JSON object with the following exact fields:\n\n" +
              "- Name (string): a short, distinct, creative name for the outfit (e.g., \"Urban Layers\", \"Midnight Chill\")\n" +
              "- Season (string): one of the following values based on the clothing in the image — \"spring\", \"summer\", \"autumn\", or \"winter\"\n" +
              "- Style (string): a single word or short phrase that captures the fashion style of the outfit (e.g., \"minimalist\", \"boho\", \"streetwear\", \"elegant\", \"grunge\")\n" +
              "- Description (string): a stylish and expressive description suitable for a social media post, like Instagram. The tone should be aesthetic, confident, and fashion-aware, focusing on the vibe, layering, textures, colors, and mood of the outfit. Use modern fashion vocabulary and keep it relevant to the season.\n\n" +
              "The description must be original, catchy, and no longer than 2-3 sentences. Avoid hashtags, emojis, and technical clothing breakdowns. Focus on the overall aesthetic and feeling.\n\n" +
              "Return ONLY the raw JSON object in one line, without markdown, explanation, or line breaks.\n\n" +
              "Example:\n" +
              "{\"Name\":\"Frosted Edge\",\"Season\":\"winter\",\"Style\":\"minimalist\",\"Description\":\"Snow tones and structured layers define this clean winter ensemble. A sleek look built for crisp air and cool confidence.\"}";

            var analysisResult = await imageManagementService.AnalyzeImageAsync<OutfitAnalysisResult>(promp, image);
            if (!analysisResult.IsSuccess)
                return Result<OutfitAnalysisResult>.Failure(analysisResult.ErrorMessage);
            return Result<OutfitAnalysisResult>.Success(analysisResult.Data);

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
