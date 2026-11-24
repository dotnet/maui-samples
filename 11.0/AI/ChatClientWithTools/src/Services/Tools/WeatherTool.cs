using Microsoft.Extensions.AI;
using System.Text.Json;
using ChatClientWithTools.Models;

namespace ChatClientWithTools.Services.Tools;

public class WeatherTool : AIFunction
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;

    public WeatherTool(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = Environment.GetEnvironmentVariable("WEATHER_API_KEY");
    }

    public override string Name => "get_weather";
    public override string Description => "Gets current weather information for a specified location";

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
                description = "Temperature units: fahrenheit, celsius, or kelvin",
                @enum = new[] { "fahrenheit", "celsius", "kelvin" },
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

        // If no API key is configured, return mock data
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            return CreateMockWeatherData(locationQuery, units, "(no API key configured)");
        }

        try
        {
            // 1. Geocode the user-provided location to get coordinates
            var geoUrl = $"https://api.openweathermap.org/geo/1.0/direct?q={Uri.EscapeDataString(locationQuery)}&limit=1&appid={_apiKey}";
            using var geoResponse = await _httpClient.GetAsync(geoUrl, cancellationToken);
            if (!geoResponse.IsSuccessStatusCode)
            {
                return CreateMockWeatherData(locationQuery, units, "(geocode lookup failed)");
            }
            var geoJson = await geoResponse.Content.ReadAsStringAsync(cancellationToken);
            var geoResults = JsonSerializer.Deserialize<List<GeocodeResult>>(geoJson) ?? new();
            var first = geoResults.FirstOrDefault();
            if (first == null)
            {
                return CreateMockWeatherData(locationQuery, units, "(location not found)");
            }

            // 2. Fetch weather by coordinates
            var unitsParam = units switch
            {
                "celsius" => "metric",
                "kelvin" => "standard",
                _ => "imperial" // default Fahrenheit
            };

            var weatherUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={first.lat}&lon={first.lon}&appid={_apiKey}&units={unitsParam}";
            using var weatherResponse = await _httpClient.GetAsync(weatherUrl, cancellationToken);
            if (!weatherResponse.IsSuccessStatusCode)
            {
                return CreateMockWeatherData(locationQuery, units, "(weather fetch failed)");
            }
            var weatherJson = await weatherResponse.Content.ReadAsStringAsync(cancellationToken);
            var weatherData = JsonSerializer.Deserialize<OpenWeatherResponse>(weatherJson);
            if (weatherData == null)
            {
                return CreateMockWeatherData(locationQuery, units, "(weather parse error)");
            }

            var unitSymbol = units switch
            {
                "celsius" => "°C",
                "kelvin" => "K",
                _ => "°F"
            };

            var displayLocation = string.IsNullOrWhiteSpace(first.state)
                ? $"{first.name}, {first.country}"
                : $"{first.name}, {first.state}, {first.country}";

            return new WeatherResult
            {
                Location = displayLocation,
                Temperature = Math.Round(weatherData.Main?.Temp ?? 0, 1),
                TemperatureUnit = unitSymbol,
                Description = weatherData.Weather?.FirstOrDefault()?.Description ?? "Unknown",
                Condition = weatherData.Weather?.FirstOrDefault()?.Main ?? "Unknown",
                Humidity = weatherData.Main?.Humidity ?? 0,
                WindSpeed = Math.Round(weatherData.Wind?.Speed ?? 0, 1),
                Icon = GetWeatherIcon(weatherData.Weather?.FirstOrDefault()?.Main ?? "Unknown")
            };
        }
        catch (Exception ex)
        {
            return CreateMockWeatherData(locationQuery, units, $"(geocode/weather error: {ex.Message})");
        }
    }

    private string GetStringArgument(AIFunctionArguments arguments, string name, string defaultValue = "")
    {
        return arguments.TryGetValue(name, out var value) ? value?.ToString() ?? defaultValue : defaultValue;
    }

    private WeatherResult CreateMockWeatherData(string location, string units, string? note = null)
    {
        // Generate somewhat realistic mock data based on location
        var random = new Random(location.GetHashCode());

        var baseTemp = location.ToLower() switch
        {
            var loc when loc.Contains("seattle") => 55,
            var loc when loc.Contains("miami") => 82,
            var loc when loc.Contains("chicago") => 48,
            var loc when loc.Contains("phoenix") => 95,
            var loc when loc.Contains("london") => 52,
            var loc when loc.Contains("tokyo") => 68,
            var loc when loc.Contains("sydney") => 72,
            _ => 65
        };

        // Convert based on units
        var temperature = units.ToLower() switch
        {
            "celsius" => (baseTemp - 32) * 5.0 / 9.0,
            "kelvin" => ((baseTemp - 32) * 5.0 / 9.0) + 273.15,
            _ => baseTemp
        };

        var unitSymbol = units.ToLower() switch
        {
            "celsius" => "°C",
            "kelvin" => "K",
            _ => "°F"
        };

        var conditions = new[] { "Clear", "Clouds", "Rain", "Snow", "Thunderstorm" };
        var condition = conditions[random.Next(conditions.Length)];

        var descriptions = condition switch
        {
            "Clear" => new[] { "clear sky", "sunny" },
            "Clouds" => new[] { "few clouds", "scattered clouds", "overcast" },
            "Rain" => new[] { "light rain", "moderate rain", "heavy rain" },
            "Snow" => new[] { "light snow", "snow", "heavy snow" },
            "Thunderstorm" => new[] { "thunderstorm", "thunderstorm with rain" },
            _ => new[] { "clear sky" }
        };

        var description = descriptions[random.Next(descriptions.Length)];
        var displayLocation = note == null ? location : $"{location} {note}";

        return new WeatherResult
        {
            Location = displayLocation,
            Temperature = Math.Round(temperature, 1),
            TemperatureUnit = unitSymbol,
            Description = description,
            Condition = condition,
            Humidity = random.Next(30, 90),
            WindSpeed = Math.Round(random.NextDouble() * 20, 1),
            Icon = GetWeatherIcon(condition)
        };
    }

    private string GetWeatherIcon(string condition)
    {
        return condition.ToLower() switch
        {
            "clear" => "☀️",
            "clouds" => "☁️",
            "rain" => "🌧️",
            "drizzle" => "🌦️",
            "thunderstorm" => "⛈️",
            "snow" => "❄️",
            "mist" or "fog" => "🌫️",
            _ => "🌤️"
        };
    }

    // OpenWeatherMap API response models
    private class GeocodeResult
    {
        public string? name { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public string? state { get; set; }
        public string? country { get; set; }
    }
    private class OpenWeatherResponse
    {
        public Weather[]? Weather { get; set; }
        public Main? Main { get; set; }
        public Wind? Wind { get; set; }
        public string? Name { get; set; }
    }

    private class Weather
    {
        public string? Main { get; set; }
        public string? Description { get; set; }
    }

    private class Main
    {
        public double Temp { get; set; }
        public double Humidity { get; set; }
    }

    private class Wind
    {
        public double Speed { get; set; }
    }
}