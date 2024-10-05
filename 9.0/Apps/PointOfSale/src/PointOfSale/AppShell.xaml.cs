using PointOfSale.Pages;

namespace PointOfSale;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        BindingContext = this;
        InitRoutes();
	}

    private void InitRoutes()
    {
        Routing.RegisterRoute(nameof(AddProductView), typeof(AddProductView));
    }

    private string selectedRoute;
    public string SelectedRoute
    {
        get { return selectedRoute; }
        set
        {
            selectedRoute = value;
            OnPropertyChanged();
        }
    }

    async void OnMenuItemChanged(System.Object sender, CheckedChangedEventArgs e)
    {
        if(!String.IsNullOrEmpty(selectedRoute))
            await Shell.Current.GoToAsync($"//{selectedRoute}");
    }
}