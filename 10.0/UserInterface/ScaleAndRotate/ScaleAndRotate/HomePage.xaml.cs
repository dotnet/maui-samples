using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ScaleAndRotate;

public class MenuItem
{
    public string Title { get; set; } = string.Empty;
    public Type PageType { get; set; } = typeof(ContentPage);
}

public partial class HomePage : ContentPage
{
    public ICommand NavigateCommand { get; private set; }
    public ObservableCollection<MenuItem> MenuItems { get; set; }

    public HomePage()
    {
        InitializeComponent();

        NavigateCommand = new Command<Type>(async (pageType) =>
        {
            await Shell.Current.GoToAsync($"///{pageType.Name}");
        });

        MenuItems = new ObservableCollection<MenuItem>
        {
            new MenuItem { Title = "Scale and Rotate (XAML)", PageType = typeof(ScaleAndRotatePage) },
            new MenuItem { Title = "Scale", PageType = typeof(ScaleDemoPage) },
            new MenuItem { Title = "ScaleX", PageType = typeof(ScaleXDemoPage) },
            new MenuItem { Title = "ScaleY", PageType = typeof(ScaleYDemoPage) },
            new MenuItem { Title = "Rotation", PageType = typeof(RotationDemoPage) },
            new MenuItem { Title = "RotationX", PageType = typeof(RotationXDemoPage) },
            new MenuItem { Title = "RotationY", PageType = typeof(RotationYDemoPage) }
        };

        BindingContext = this;
    }
}
