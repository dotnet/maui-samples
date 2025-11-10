namespace SampleHybridWebViewApp;

public partial class MainPage : ContentPage
{
	private readonly WeatherProxy _weatherProxy;

	public MainPage(WeatherProxy weatherProxy)
	{
		InitializeComponent();
		_weatherProxy = weatherProxy;
		
		// Intercept web requests to load HTML from app package
		hybridWebView.WebResourceRequested += OnWebResourceRequested;
		
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

	private async void OnWebResourceRequested(object? sender, WebViewWebResourceRequestedEventArgs e)
	{
		// Handle the root request and index.html
		if (e.Uri.ToString() == "app://0.0.0.1/" || e.Uri.ToString().EndsWith("index.html"))
		{
			e.Handled = true;
			
			try
			{
				// Read the HTML file from the app package with wwwroot prefix
				using var stream = await FileSystem.OpenAppPackageFileAsync("wwwroot/index.html");
				using var reader = new StreamReader(stream);
				var html = await reader.ReadToEndAsync();
				
				// Return the HTML
				var responseStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(html));
				e.SetResponse(200, "OK", "text/html", responseStream);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[MAUI] Failed to load HTML: {ex.Message}");
				e.Handled = false;
			}
		}
		// Handle JavaScript file
		else if (e.Uri.ToString().Contains("HybridWebView.js"))
		{
			e.Handled = true;
			
			try
			{
				using var stream = await FileSystem.OpenAppPackageFileAsync("wwwroot/HybridWebView.js");
				using var reader = new StreamReader(stream);
				var js = await reader.ReadToEndAsync();
				
				var responseStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(js));
				e.SetResponse(200, "OK", "application/javascript", responseStream);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[MAUI] Failed to load JS: {ex.Message}");
				e.Handled = false;
			}
		}
		// Handle other resources like images - they're requested without wwwroot prefix from HTML
		else if (e.Uri.ToString().Contains("dotnet_bot.png"))
		{
			e.Handled = true;
			
			try
			{
				using var stream = await FileSystem.OpenAppPackageFileAsync("wwwroot/dotnet_bot.png");
				var memoryStream = new MemoryStream();
				await stream.CopyToAsync(memoryStream);
				memoryStream.Position = 0;
				
				e.SetResponse(200, "OK", "image/png", memoryStream);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[MAUI] Failed to load resource: {ex.Message}");
				e.Handled = false;
			}
		}
	}
}
