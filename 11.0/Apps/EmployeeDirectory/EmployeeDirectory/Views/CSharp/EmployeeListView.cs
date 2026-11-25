using Microsoft.Maui.Controls.Shapes;
using EmployeeDirectory.Core.Data;
using EmployeeDirectory.Core.ViewModels;

namespace EmployeeDirectory.Views.CSharp;
public class EmployeeListView : ContentPage
{
    private FavoritesViewModel? viewModel;
    private IFavoritesRepository? favoritesRepository;
    private CollectionView collectionView;
    private ToolbarItem toolbarItem;
    private Button fabButton;

    public EmployeeListView()
    {
        Title = "Employee Directory";
        BackgroundColor = Application.Current?.Resources["Gray100"] as Color ?? Color.FromArgb("#F2F2F7");

        toolbarItem = new ToolbarItem("search", "Search.png", () =>
        {
            var search = new SearchListView();
            Navigation.PushAsync(search);
        }, 0, 0);

        ToolbarItems.Add(toolbarItem);

        collectionView = new CollectionView
        {
            IsGrouped = true,
            SelectionMode = SelectionMode.Single,
            BackgroundColor = Colors.Transparent,
            ItemTemplate = new DataTemplate(() =>
            {
                var border = new Border
                {
                    BackgroundColor = Application.Current?.Resources["White"] as Color ?? Colors.White,
                    Stroke = Application.Current?.Resources["Gray300"] as Color ?? Color.FromArgb("#D1D1D6"),
                    StrokeShape = new RoundRectangle { CornerRadius = 12 },
                    Padding = 16,
                    Margin = new Thickness(16, 8),
                    InputTransparent = false
                };

                var avatarBorder = new Border
                {
                    WidthRequest = 60,
                    HeightRequest = 60,
                    StrokeShape = new Ellipse(),
                    Stroke = Application.Current?.Resources["Gray300"] as Color ?? Color.FromArgb("#D1D1D6"),
                    BackgroundColor = Application.Current?.Resources["Gray100"] as Color ?? Color.FromArgb("#F2F2F7"),
                    InputTransparent = true
                };
                var avatar = new Image { Aspect = Aspect.AspectFill };
                avatar.SetBinding(Image.SourceProperty, "Photo");
                avatarBorder.Content = avatar;

                var nameLabel = new Label
                {
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Application.Current?.Resources["Gray900"] as Color ?? Color.FromArgb("#1D1D1F")
                };
                nameLabel.SetBinding(Label.TextProperty, "Name");

                var titleLabel = new Label
                {
                    FontSize = 14,
                    TextColor = Application.Current?.Resources["Gray600"] as Color ?? Color.FromArgb("#8E8E93"),
                    Margin = new Thickness(0, 2, 0, 0)
                };
                titleLabel.SetBinding(Label.TextProperty, "Title");

                var infoStack = new VerticalStackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Spacing = 0,
                    Children = { nameLabel, titleLabel }
                };

                var favLabel = new Label
                {
                    Text = "♥",
                    FontSize = 20,
                    TextColor = Application.Current?.Resources["Primary"] as Color ?? Color.FromArgb("#0066CC"),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                favLabel.SetBinding(Label.IsVisibleProperty, "IsFavorite");

                var grid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                            new ColumnDefinition{ Width = new GridLength(60) },
                            new ColumnDefinition{ Width = GridLength.Star },
                            new ColumnDefinition{ Width = GridLength.Auto }
                    },
                    ColumnSpacing = 12
                };
                grid.Add(avatarBorder, 0, 0);
                grid.Add(infoStack, 1, 0);
                grid.Add(favLabel, 2, 0);

                border.Content = grid;
                return border;
            })
        };
        collectionView.SelectionChanged += OnSelectionChanged;

        // Floating Action Button (FAB)
        fabButton = new Button
        {
            Text = "🔍",
            FontSize = 20,
            WidthRequest = 56,
            HeightRequest = 56,
            CornerRadius = 28,
            BackgroundColor = Application.Current?.Resources["Primary"] as Color ?? Color.FromArgb("#0066CC"),
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(16),
            Shadow = new Shadow
            {
                Brush = Brush.Black,
                Opacity = 0.3f,
                Radius = 8,
                Offset = new Microsoft.Maui.Graphics.Point(0, 4)
            }
        };
        fabButton.Clicked += (s, e) =>
        {
            var search = new SearchListView();
            Navigation.PushAsync(search);
        };

        // Layout
        var gridLayout = new Grid();
        gridLayout.Children.Add(collectionView);
        gridLayout.Children.Add(fabButton);

        Content = gridLayout;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        if (LoginViewModel.ShouldShowLogin(App.LastUseTime))
            await Navigation.PushModalAsync(new LoginView());

        favoritesRepository = await XmlFavoritesRepository.OpenIsolatedStorage("XamarinFavorites.json");

        if (favoritesRepository.GetAll().Count() == 0)
            favoritesRepository = await XmlFavoritesRepository.OpenFile("XamarinFavorites.json");

        viewModel = new FavoritesViewModel(favoritesRepository, true);
        collectionView.ItemsSource = viewModel.Groups;
    }

    private async void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Person person)
        {
            var selectedEmployee = new EmployeeView
            {
                BindingContext = new PersonViewModel(person, favoritesRepository)
            };
            await Navigation.PushAsync(selectedEmployee);
            collectionView.SelectedItem = null;
        }
    }
}
