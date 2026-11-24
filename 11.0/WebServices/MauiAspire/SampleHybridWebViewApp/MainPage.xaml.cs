namespace SampleHybridWebViewApp;

public partial class MainPage : ContentPage
{
	private readonly WeatherProxy _weatherProxy;

	public MainPage(WeatherProxy weatherProxy)
	{
		InitializeComponent();
		_weatherProxy = weatherProxy;
		
		// Handle messages from JavaScript
		hybridWebView.RawMessageReceived += async (sender, args) =>
		{
			try
			{
				System.Diagnostics.Debug.WriteLine($"Received message: {args.Message}");
				
				if (args.Message == "GetWeatherForecast")
				{
					var result = await _weatherProxy.GetWeatherForecast();
					System.Diagnostics.Debug.WriteLine($"Weather data: {result}");
					
					// Use proper JavaScript invocation with properly escaped JSON
					await hybridWebView.InvokeJavaScriptAsync($"window.receiveWeatherData({result})");
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error handling message: {ex.Message}");
				System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
			}
		};
	}
}
