using EmployeeDirectory.Core.Data;
using EmployeeDirectory.Core.ViewModels;
using EmployeeDirectory.Core.Services;

namespace EmployeeDirectory.Views.Xaml;

public partial class EmployeeListXaml : ContentPage
{
    private FavoritesViewModel viewModel;
    private IFavoritesRepository favoritesRepository;

    public EmployeeListXaml()
    {
        InitializeComponent();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            // Load persisted favorites (JSON storage)
            favoritesRepository = await XmlFavoritesRepository.OpenIsolatedStorage("XamarinFavorites.json");
            if (!favoritesRepository.GetAll().Any())
                favoritesRepository = await XmlFavoritesRepository.OpenFile("XamarinFavorites.json");

            // Create view model and set BindingContext BEFORE relying on XAML bindings
            viewModel = new FavoritesViewModel(favoritesRepository, true);
            BindingContext = viewModel;

            // Explicitly re-assign ItemsSource in case initial binding occurred before BindingContext set
            if (collectionView != null)
                collectionView.ItemsSource = viewModel.Groups;

            System.Diagnostics.Debug.WriteLine($"[EmployeeListXaml] CollectionView path active. Groups={viewModel.Groups.Count} People={viewModel.Groups.Sum(g=>g.People.Count)}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EmployeeListXaml] ERROR: {ex}");
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Person person)
        {
            var employeeView = new EmployeeXaml
            {
                BindingContext = new PersonViewModel(person, favoritesRepository)
            };
            await Navigation.PushAsync(employeeView);
            collectionView.SelectedItem = null; // clear selection
        }
    }
}
