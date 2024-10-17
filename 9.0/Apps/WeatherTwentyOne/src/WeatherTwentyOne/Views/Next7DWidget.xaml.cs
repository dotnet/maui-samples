using WeatherTwentyOne.ViewModels;

namespace WeatherTwentyOne.Views;

public partial class Next7DWidget
{
    public Next7DWidget()
    {
        InitializeComponent();

        BindingContext = new HomeViewModel();
    }
}
