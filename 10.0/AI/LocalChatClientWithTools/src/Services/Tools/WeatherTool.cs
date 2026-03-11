using Microsoft.Extensions.AI;
using System.Text.Json;
using System.Text.Json.Serialization;
using LocalChatClientWithTools.Models;

namespace LocalChatClientWithTools.Services.Tools;

public class WeatherTool : AIFunction
{
    private readonly HttpClient _httpClient;

    public WeatherTool(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public override string Name => "get_weather";
    public override string Description => "Gets current weather information for a specified location using the free open-meteo.com API (no API key required)";

    public override JsonElement JsonSchema => JsonSerializer.SerializeToElement(new
    {
        type = "object",
        properties = new
        {
            location = new
            {
                type = "string",
                description = "The city or location to get weather for (e.g., 'Seattle', 'London, UK')"
            },
            units = new
            {
                type = "string",
                description = "Temperature units: fahrenheit or celsius",
                @enum = new[] { "fahrenheit", "celsius" },
                @default = "fahrenheit"
            }
        },
        required = new[] { "location" }
    });

    protected override async ValueTask<object?> InvokeCoreAsync(AIFunctionArguments arguments, CancellationToken cancellationToken)
    {
        var locationQuery = GetStringArgument(arguments, "location");
        var units = GetStringArgument(arguments, "units", "fahrenheit").ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(locationQuery))
        {
            throw new ArgumentException("Location cannot be empty");
        }

        try
        {
            // 1. Geocode using open-meteo geocoding API (free, no key)
            var geoUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(locationQuery)}&count=1";
            using var geoResponse = await _httpClient.GetAsync(geoUrl, cancellationToken);
            geoResponse.EnsureSuccessStatusCode();

            var geoJson = await geoResponse.Content.ReadAsStringAsync(cancellationToken);
            var geoData = JsonSerializer.Deserialize<GeocodingResponse>(geoJson);
            var location = geoData?.Results?.FirstOrDefault();

            if (location is null)
            {
                return new WeatherResult
                {
                    Location = locationQuery,
                    Temperature = 0,
                    TemperatureUnit = units == "celsius" ? "°C" : "°F",
                    Description = "Location not found",
                    Condition = "Unknown",
                    Humidity = 0,
                    WindSpeed = 0,
                    Icon = "❓"
                };
            }

            // 2. Fetch current weather from open-meteo forecast API (free, no key)
            var tempUnit = units == "celsius" ? "celsius" : "fahrenheit";
            var windUnit = units == "celsius" ? "kmh" : "mph";
            var weatherUrl = $"https://api.open-meteo.com/v1/forecast?latitude={location.Latitude:F4}&longitude={location.Longitude:F4}" +
                $"&current=temperature_2m,relative_humidity_2m,weather_code,wind_speed_10m" +
                $"&temperature_unit={tempUnit}&wind_speed_unit={windUnit}";

            using var weatherResponse = await _httpClient.GetAsync(weatherUrl, cancellationToken);
            weatherResponse.EnsureSuccessStatusCode();

            var weatherJson = await weatherResponse.Content.ReadAsStringAsync(cancellationToken);
            var weatherData = JsonSerializer.Deserialize<OpenMeteoResponse>(weatherJson);
            var current = weatherData?.Current;

            if (current is null)
            {
                return new WeatherResult
                {
                    Location = FormatLocation(location),
                    Temperature = 0,
                    TemperatureUnit = units == "celsius" ? "°C" : "°F",
                    Description = "Weather data unavailable",
                    Condition = "Unknown",
                    Humidity = 0,
                    WindSpeed = 0,
                    Icon = "❓"
                };
            }

            var unitSymbol = units == "celsius" ? "°C" : "°F";
            var (condition, description, icon) = GetWeatherInfo(current.WeatherCode);

            return new WeatherResult
            {
                Location = FormatLocation(location),
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
                Location = $"{locationQuery} (error: {ex.Message})",
                Temperature = 0,
                TemperatureUnit = units == "celsius" ? "°C" : "°F",
                Description = "Weather fetch failed",
                Condition = "Unknown",
                Humidity = 0,
                WindSpeed = 0,
                Icon = "⚠️"
            };
        }
    }

    private static string GetStringArgument(AIFunctionArguments arguments, string name, string defaultValue = "")
    {
        return arguments.TryGetValue(name, out var value) ? value?.ToString() ?? defaultValue : defaultValue;
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

    // open-meteo API response models
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
}