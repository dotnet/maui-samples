using System.Text.Json;

namespace FlexLayoutDemos.Views
{
    class ImageList
    {
        public List<string> Photos { get; set; }
    }

    public partial class PhotoWrappingPage : ContentPage
    {
        HttpClient httpClient;
        JsonSerializerOptions serializerOptions;

        public PhotoWrappingPage ()
        {
            InitializeComponent ();

            httpClient = new HttpClient();
            serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            LoadBitmapCollection();
        }

        async void LoadBitmapCollection()
        {
            try
            {
                Uri uri = new Uri("https://raw.githubusercontent.com/xamarin/docs-archive/master/Images/stock/small/stock.json");
                HttpResponseMessage response = await httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    ImageList photos = JsonSerializer.Deserialize<ImageList>(content, serializerOptions);

                    // Create an Image object for each bitmap
                    foreach (string photoUri in photos.Photos)
                    {
                        Image image = new Image
                        {
                            Source = ImageSource.FromUri(new Uri(photoUri)),
                            HeightRequest = 100,
                            WidthRequest = 100
                        };
                        flexLayout.Children.Add(image);
                    }
                }
            }
            catch
            {
                flexLayout.Children.Add(new Label
                {
                    Text = "Cannot access list of bitmap files"
                });
            }

            activityIndicator.IsRunning = false;
            activityIndicator.IsVisible = false;
        }
    }
}
