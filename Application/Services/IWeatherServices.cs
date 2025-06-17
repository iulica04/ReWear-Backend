namespace Application.Services
{
    public interface IWeatherServices
    {
        Task<string> GetWeatherAsync(string lon, string lat);
    }
}
