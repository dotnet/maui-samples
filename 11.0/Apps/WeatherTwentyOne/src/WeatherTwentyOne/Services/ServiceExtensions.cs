namespace WeatherTwentyOne;

public static class ServiceProvider
{
    public static TService GetService<TService>()
        => Current.GetService<TService>();

    public static IServiceProvider Current
        => IPlatformApplication.Current.Services;
}
