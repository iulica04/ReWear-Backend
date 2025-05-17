using Application.Models;
using Domain.Common;

namespace Application.Services
{
    public interface  IClothingItemService
    {
        Task<Result<ImageAnalysisResult>> AnalyzeClothingItemsAsync(byte[] imageFront, byte[]? imageBack);
        Task<Result<bool>> DeleteImageAsync(string fileName);
        Task<List<string>> ListImagesAsync();
        Task<List<string>> ListImageBase64Async();
        Task<Result<string>> UploadImageAsync(byte[] image,
            string userId,
            string entityId,
            string entityType,
            string imageType,
            string extension = "jpg");
    }
}
