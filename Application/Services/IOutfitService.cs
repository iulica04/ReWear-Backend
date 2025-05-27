using Application.Models;
using Domain.Common;

namespace Application.Services
{
    public interface IOutfitService
    {
        Task<Result<List<OutfitItemsAnalysisResult>>> AnalyzeClothingItemsAsync(byte[] image);
        Task<Result<OutfitAnalysisResult>> AnalyzeOutfitAsync(byte[] image);
        Task<Result<bool>> DeleteImageAsync(string fileName);
        Task<List<string>> ListImagesAsync();
        Task<Result<string>> UploadImageAsync(byte[] image,
            string userId,
            string entityId,
            string entityType,
            string imageType,
            string extension = "jpg");

    }
}
