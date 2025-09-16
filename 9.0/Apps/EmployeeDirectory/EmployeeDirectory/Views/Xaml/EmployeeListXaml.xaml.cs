namespace EmployeeDirectory.Views.Xaml;

public partial class EmployeeListXaml : ContentPage
{
    private FavoritesViewModel viewModel;
    private IFavoritesRepository favoritesRepository;
    private ToolbarItem toolbarItem;

    public EmployeeListXaml()
    {
        InitializeComponent();

        toolbarItem = new ToolbarItem("search", "Search.png", () =>
        {
            var search = new SearchListXaml();
            Navigation.PushAsync(search);
        }, 0, 0);

        ToolbarItems.Add(toolbarItem);
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        if (LoginViewModel.ShouldShowLogin(App.LastUseTime))
            await Navigation.PushModalAsync(new LoginXaml());

        favoritesRepository = await XmlFavoritesRepository.OpenIsolatedStorage("XamarinFavorites.json");

        if (favoritesRepository.GetAll().Count() == 0)
                favoritesRepository = await XmlFavoritesRepository.OpenFile("XamarinFavorites.json");

        viewModel = new FavoritesViewModel(favoritesRepository, true);

        collectionView.ItemsSource = viewModel.Groups;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // The CollectionView is grouped, so e.CurrentSelection will contain a Person, but the binding context for each item is a Person, not a PeopleGroup.
        // However, the default ItemsSource for grouped CollectionView expects each group to expose an IEnumerable property named 'Items' or 'Children'.
        // Our PeopleGroup exposes 'People', so we need to set the ItemsSource property in XAML:
        // <CollectionView.ItemsSource> <Binding Path="Groups" /> </CollectionView.ItemsSource>
        //
        // But for now, let's add a debug log to see if this is called at all.
        System.Diagnostics.Debug.WriteLine($"SelectionChanged invoked. Selection count: {e.CurrentSelection.Count}");
        if (e.CurrentSelection.FirstOrDefault() is Person person)
        {
            var employeeView = new EmployeeXaml
            {
                BindingContext = new PersonViewModel(person, favoritesRepository)
            };

            await Navigation.PushAsync(employeeView);
            
            // Clear selection
            collectionView.SelectedItem = null;
        }
    }

    private async void OnFabClicked(object sender, EventArgs e)
    {
        var search = new SearchListXaml();
        await Navigation.PushAsync(search);
    }
}
