using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SampleHybridWebViewApp;

public class WeatherForecast
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string? Summary { get; set; }
}

[JsonSourceGenerationOptions(WriteIndented = false)]
[JsonSerializable(typeof(WeatherForecast[]))]
internal partial class WeatherJsonContext : JsonSerializerContext
{
}

public class WeatherProxy
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WeatherProxy(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> GetWeatherForecast()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("weatherapi");
            
            // Make request to the weather API via service discovery
            var response = await client.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast");
            
            if (response == null)
            {
                return JsonSerializer.Serialize(Array.Empty<WeatherForecast>(), WeatherJsonContext.Default.WeatherForecastArray);
            }

            return JsonSerializer.Serialize(response, WeatherJsonContext.Default.WeatherForecastArray);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting weather: {ex.Message}");
            return JsonSerializer.Serialize(Array.Empty<WeatherForecast>(), WeatherJsonContext.Default.WeatherForecastArray);
        }
    }
}
