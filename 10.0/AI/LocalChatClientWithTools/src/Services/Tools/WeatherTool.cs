using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI;

namespace LocalChatClientWithTools.Services.Tools;

public partial class WeatherTool(HttpClient httpClient)
{
    public static AIFunction CreateAIFunction(IServiceProvider services)
        => AIFunctionFactory.Create(
            services.GetRequiredService<WeatherTool>().GetWeatherAsync,
            name: "get_weather",
            serializerOptions: ToolJsonContext.Default.Options);

    [Description("Gets current weather information for a specified location using the free open-meteo.com API (no API key required)")]
    public async Task<WeatherResult> GetWeatherAsync(
        [Description("The city or location to get weather for (e.g., 'Seattle', 'Cape Town')")] string location,
        [Description("Temperature units")] TemperatureUnit units = TemperatureUnit.Fahrenheit,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be empty");

        try
        {
            var geoUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(location)}&count=1";
            using var geoResponse = await httpClient.GetAsync(geoUrl, cancellationToken);
            geoResponse.EnsureSuccessStatusCode();

            var geoJson = await geoResponse.Content.ReadAsStringAsync(cancellationToken);
            var geoData = JsonSerializer.Deserialize(geoJson, WeatherApiJsonContext.Default.GeocodingResponse);
            var geoResult = geoData?.Results?.FirstOrDefault();

            if (geoResult is null)
            {
                return new WeatherResult
                {
                    Location = location,
                    Temperature = 0,
                    TemperatureUnit = units == TemperatureUnit.Celsius ? "°C" : "°F",
                    Description = "Location not found",
                    Condition = "Unknown",
                    Humidity = 0,
                    WindSpeed = 0,
                    Icon = "❓"
                };
            }

            var tempUnit = units == TemperatureUnit.Celsius ? "celsius" : "fahrenheit";
            var windUnit = units == TemperatureUnit.Celsius ? "kmh" : "mph";
            var weatherUrl = $"https://api.open-meteo.com/v1/forecast?latitude={geoResult.Latitude:F4}&longitude={geoResult.Longitude:F4}" +
                $"&current=temperature_2m,relative_humidity_2m,weather_code,wind_speed_10m" +
                $"&temperature_unit={tempUnit}&wind_speed_unit={windUnit}";

            using var weatherResponse = await httpClient.GetAsync(weatherUrl, cancellationToken);
            weatherResponse.EnsureSuccessStatusCode();

            var weatherJson = await weatherResponse.Content.ReadAsStringAsync(cancellationToken);
            var weatherData = JsonSerializer.Deserialize(weatherJson, WeatherApiJsonContext.Default.OpenMeteoResponse);
            var current = weatherData?.Current;

            if (current is null)
            {
                return new WeatherResult
                {
                    Location = FormatLocation(geoResult),
                    Temperature = 0,
                    TemperatureUnit = units == TemperatureUnit.Celsius ? "°C" : "°F",
                    Description = "Weather data unavailable",
                    Condition = "Unknown",
                    Humidity = 0,
                    WindSpeed = 0,
                    Icon = "❓"
                };
            }

            var unitSymbol = units == TemperatureUnit.Celsius ? "°C" : "°F";
            var (condition, description, icon) = GetWeatherInfo(current.WeatherCode);

            return new WeatherResult
            {
                Location = FormatLocation(geoResult),
                Temperature = Math.Round(current.Temperature, 1),
                TemperatureUnit = unitSymbol,
                Description = description,
                Condition = condition,
                Humidity = current.Humidity,
                WindSpeed = Math.Round(current.WindSpeed, 1),
                Icon = icon
            };
        }
        catch (Exception ex)
        {
            return new WeatherResult
            {
                Location = $"{location} (error: {ex.Message})",
                Temperature = 0,
                TemperatureUnit = units == TemperatureUnit.Celsius ? "°C" : "°F",
                Description = "Weather fetch failed",
                Condition = "Unknown",
                Humidity = 0,
                WindSpeed = 0,
                Icon = "⚠️"
            };
        }
    }

    private static string FormatLocation(GeocodingResult location)
    {
        var parts = new List<string> { location.Name ?? "Unknown" };
        if (!string.IsNullOrEmpty(location.Admin1))
            parts.Add(location.Admin1);
        if (!string.IsNullOrEmpty(location.Country))
            parts.Add(location.Country);
        return string.Join(", ", parts);
    }

    private static (string Condition, string Description, string Icon) GetWeatherInfo(int code)
    {
        return code switch
        {
            0 => ("Clear", "clear sky", "☀️"),
            1 => ("Clear", "mainly clear", "🌤️"),
            2 => ("Clouds", "partly cloudy", "⛅"),
            3 => ("Clouds", "overcast", "☁️"),
            45 or 48 => ("Fog", "foggy", "🌫️"),
            51 or 53 or 55 => ("Drizzle", "drizzle", "🌦️"),
            61 or 63 or 65 => ("Rain", "rain", "🌧️"),
            66 or 67 => ("Rain", "freezing rain", "🌧️"),
            71 or 73 or 75 => ("Snow", "snow", "❄️"),
            77 => ("Snow", "snow grains", "❄️"),
            80 or 81 or 82 => ("Rain", "rain showers", "🌧️"),
            85 or 86 => ("Snow", "snow showers", "❄️"),
            95 => ("Thunderstorm", "thunderstorm", "⛈️"),
            96 or 99 => ("Thunderstorm", "thunderstorm with hail", "⛈️"),
            _ => ("Unknown", $"weather code {code}", "🌤️")
        };
    }

    [JsonConverter(typeof(JsonStringEnumConverter<TemperatureUnit>))]
    public enum TemperatureUnit
    {
        Fahrenheit,
        Celsius
    }

    public record WeatherResult
    {
        public required string Location { get; init; }
        public required double Temperature { get; init; }
        public required string TemperatureUnit { get; init; }
        public required string Description { get; init; }
        public required string Condition { get; init; }
        public required double Humidity { get; init; }
        public required double WindSpeed { get; init; }
        public string? Icon { get; init; }
    }

    // open-meteo API response models (private, AOT-safe via nested source gen)
    private class GeocodingResponse
    {
        [JsonPropertyName("results")]
        public List<GeocodingResult>? Results { get; set; }
    }

    private class GeocodingResult
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
        [JsonPropertyName("admin1")]
        public string? Admin1 { get; set; }
        [JsonPropertyName("country")]
        public string? Country { get; set; }
    }

    private class OpenMeteoResponse
    {
        [JsonPropertyName("current")]
        public CurrentWeather? Current { get; set; }
    }

    private class CurrentWeather
    {
        [JsonPropertyName("temperature_2m")]
        public double Temperature { get; set; }
        [JsonPropertyName("relative_humidity_2m")]
        public double Humidity { get; set; }
        [JsonPropertyName("weather_code")]
        public int WeatherCode { get; set; }
        [JsonPropertyName("wind_speed_10m")]
        public double WindSpeed { get; set; }
    }

    [JsonSerializable(typeof(GeocodingResponse))]
    [JsonSerializable(typeof(OpenMeteoResponse))]
    private partial class WeatherApiJsonContext : JsonSerializerContext;
}
