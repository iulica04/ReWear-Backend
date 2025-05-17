using Domain.Common;


namespace Application.Services
{
    public interface IImageManagementService
    {
        Task<Result<T>> AnalyzeImageAsync<T>(string prompt, params byte[][] images);
        Task<Result<List<T>>> AnalyzeImageListAsync<T>(string prompt, params byte[][] images);
        Task<Result<string>> UploadImageAsync(byte[] image,
            string userId,
            string entityId,
            string entityType,
            string imageType,
            string extension = "jpg");
        Task<bool> ImageExistsAsync(string fileName);
        Task<Result<bool>> DeleteImageAsync(string fileName);
        Task<List<string>> ListImagesAsync();
        Task<List<string>> ListImageBase64Async();
    }
}
