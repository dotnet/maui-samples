namespace Weather;

// MainPage represents the main screen of the weather app
public partial class MainPage : ContentPage
{
    // Service used to fetch weather data from API
    RestService _restService;

    // Constructor required to initializes UI components and services
    public MainPage()
    {
        InitializeComponent();

        // Create instance of RestService
        _restService = new RestService();
    }

    // This method runs when the "Get Weather" button is clicked
    async void OnGetWeatherButtonClicked(object sender, EventArgs e)
    {
        // Check if API key is missing or not set
        if (string.IsNullOrWhiteSpace(Constants.OpenWeatherMapAPIKey) || Constants.OpenWeatherMapAPIKey == "YOUR_API_KEY")
        {
            await DisplayAlertAsync(
                "API Key Missing",
                "Please set your OpenWeatherMap API key in Constants.OpenWeatherMapAPIKey.",
                "OK"
            );
            return;
        }

        // Ensure user has entered a city name or its left blank
        if (!string.IsNullOrWhiteSpace(_cityEntry.Text))
        {
            // Build request URL
            string requestUri = GenerateRequestUri(Constants.OpenWeatherMapEndpoint);

            // Call API and get weather data
            WeatherData? weatherData = await _restService.GetWeatherData(requestUri);

            // Bind data to UI
            BindingContext = weatherData;
        }
    }

    // Builds the API request URL with city, units, and API key
    string GenerateRequestUri(string endpoint)
    {
        string requestUri = endpoint;

        // Add city name to query
        requestUri += $"?q={_cityEntry.Text}";

        // Set temperature unit (imperial = Fahrenheit, metric = Celsius)
        requestUri += "&units=imperial";

        // Add API key for authentication
        requestUri += $"&appid={Constants.OpenWeatherMapAPIKey}";

        return requestUri;
    }
}
