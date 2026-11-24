using WeatherTwentyOne.ViewModels;

namespace WeatherTwentyOne.Views;

public partial class Next24HrWidget
{
    public Next24HrWidget()
    {
        InitializeComponent();

        BindingContext = new HomeViewModel();
    }
}
