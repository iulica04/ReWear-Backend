using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Services;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class WeatherServices : IWeatherServices
    {
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;

        public WeatherServices(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenWeatherMap:Key"];
        }

        public async Task<string> GetWeatherAsync(string lon, string lat)
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&units=metric&appid={_apiKey}";
                   
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return "Error retrieving weather data.";

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var root = doc.RootElement;

            var temp = root.GetProperty("main").GetProperty("temp").GetDecimal();
            var humidity = root.GetProperty("main").GetProperty("humidity").GetInt32();
            var pressure = root.GetProperty("main").GetProperty("pressure").GetInt32();
            var wind = root.GetProperty("wind").GetProperty("speed").GetDecimal();
            var description = root.GetProperty("weather")[0].GetProperty("description").GetString();

            return $"Current weather: {description}\nTemperature: {temp}°C\nHumidity: {humidity}%\nPressure: {pressure} hPa\nWind: {wind} m/s";
        }
    }
}
