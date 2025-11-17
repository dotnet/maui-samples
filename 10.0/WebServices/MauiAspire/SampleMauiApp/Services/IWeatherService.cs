namespace SampleMauiApp.Services;

public interface IWeatherService
{
    Task<WeatherForecast[]> GetWeatherForecastAsync();
}
