namespace Weather;

public partial class MainPage : ContentPage
{
    RestService _restService;

    public MainPage()
    {
        InitializeComponent();
        _restService = new RestService();
    }

    async void OnGetWeatherButtonClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Constants.OpenWeatherMapAPIKey) || Constants.OpenWeatherMapAPIKey == "YOUR_API_KEY")
        {
            await DisplayAlertAsync("API Key Missing", "Please set your OpenWeatherMap API key in Constants.OpenWeatherMapAPIKey.", "OK");
            return;
        }
        if (!string.IsNullOrWhiteSpace(_cityEntry.Text))
        {
            string requestUri = GenerateRequestUri(Constants.OpenWeatherMapEndpoint);
            WeatherData? weatherData = await _restService.GetWeatherData(requestUri);
            BindingContext = weatherData;
        }
    }

    string GenerateRequestUri(string endpoint)
    {
        string requestUri = endpoint;
        requestUri += $"?q={_cityEntry.Text}";
        requestUri += "&units=imperial"; // or units=metric
        requestUri += $"&appid={Constants.OpenWeatherMapAPIKey}"; // changed to lowercase 'appid'
        return requestUri;
    }
}
