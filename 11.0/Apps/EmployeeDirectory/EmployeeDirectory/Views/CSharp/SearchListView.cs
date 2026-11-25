using EmployeeDirectory.Core.Data;
using EmployeeDirectory.Core.ViewModels;
using EmployeeDirectory.Core.Services;

namespace EmployeeDirectory.Views.CSharp;

public class SearchListView : ContentPage
{
    private Search search;
    private SearchViewModel viewModel;
    private IFavoritesRepository favoritesRepository;
    private CollectionView collectionView;

    public SearchListView()
    {
        InitializeViewModel();
        var searchEntry = new Entry { Placeholder = "Search For" };
        searchEntry.SetBinding(Entry.TextProperty, "SearchText");
        searchEntry.TextChanged += OnValueChanged;

        collectionView = new CollectionView
        {
            IsGrouped = true,
            SelectionMode = SelectionMode.Single,
            ItemsSource = viewModel.Groups,
            GroupHeaderTemplate = new DataTemplate(() =>
            {
                var label = new Label { VerticalTextAlignment = TextAlignment.Center, Padding = new Thickness(5,0,0,0) };
                label.SetBinding(Label.TextProperty, "Title");
                return label;
            }),
            ItemTemplate = new DataTemplate(() =>
            {
                var grid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition{ Width = 44 }, new ColumnDefinition{ Width = GridLength.Star } }, Padding = new Thickness(0,4) };
                var image = new Image { WidthRequest = 44, HeightRequest = 44 };
                image.SetBinding(Image.SourceProperty, "Photo");
                var name = new Label();
                name.SetBinding(Label.TextProperty, "Name");
                var title = new Label { FontSize = 10 };
                title.SetBinding(Label.TextProperty, "Title");
                var stack = new VerticalStackLayout { Spacing = 0, Padding = new Thickness(5,0,0,0) };
                stack.Add(name);
                stack.Add(title);
                grid.Add(image);
                grid.Add(stack);
                Grid.SetColumn(stack,1);
                return grid;
            })
        };

        collectionView.SelectionChanged += OnSelectionChanged;
        Content = new Grid
        {
            RowDefinitions = new RowDefinitionCollection { new RowDefinition{ Height = GridLength.Auto }, new RowDefinition{ Height = GridLength.Star } },
            Children = { searchEntry, collectionView }
        };
        Grid.SetRow(collectionView,1);

        Title = "Search";
    }

    private void InitializeViewModel()
    {
        search = new Search(string.Empty);
        viewModel = new SearchViewModel(App.Service, search);

        viewModel.SearchCompleted += OnSearchCompleted;
        viewModel.Error += async (sender, e) =>
        {
            await DisplayAlertAsync("Help", e.Exception.Message, "OK");
        };

        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // Initialize favorites repository asynchronously
        if (favoritesRepository == null)
        {
            favoritesRepository = await XmlFavoritesRepository.OpenFile("XamarinFavorites.json");
        }
    }

    private void OnSearchCompleted(object? sender, SearchCompletedEventArgs e)
    {
        collectionView.ItemsSource = viewModel.Groups ?? new ObservableCollection<PeopleGroup>();
    }

    private void OnValueChanged(object? sender, TextChangedEventArgs e)
    {
        viewModel.Search();
    }

    private async void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Person person)
        {
            var employeeView = new EmployeeView
            {
                BindingContext = new PersonViewModel(person, favoritesRepository)
            };
            await Navigation.PushAsync(employeeView);
            collectionView.SelectedItem = null;
        }
    }
}