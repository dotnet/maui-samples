using System.Diagnostics;
using MMC = Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using EmployeeDirectory.Core.ViewModels;
using EmployeeDirectory.Core.Utilities;

namespace EmployeeDirectory.Views.CSharp;

public class EmployeeView : ContentPage
{
    private const int IMAGE_SIZE = 150;
    private Label favoriteLabel;
    private MMC.Switch favoriteSwitch;
    private Image photo;
    private CollectionView propertiesView;

    public EmployeeView()
    {
        // Profile Image in a circular border
        var photoBorder = new Border
        {
            StrokeShape = new Ellipse(),
            StrokeThickness = 0,
            BackgroundColor = Colors.Transparent,
            HorizontalOptions = LayoutOptions.Center,
            Content = (photo = new Image { WidthRequest = IMAGE_SIZE, HeightRequest = IMAGE_SIZE, Aspect = Aspect.AspectFill, Source = "DetailsPlaceholder.jpg" })
        };

        // Name, Title, Favorite
        favoriteLabel = new Label { FontSize = 14 };
        favoriteSwitch = new MMC.Switch();
        var nameLabel = new Label { FontSize = 24, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center };
        nameLabel.SetBinding(Label.TextProperty, "Person.Name");
        var titleLabel = new Label { FontSize = 16, TextColor = Colors.Gray, HorizontalOptions = LayoutOptions.Center };
        titleLabel.SetBinding(Label.TextProperty, "Person.Title");
        var heartIcon = new Image { Source = "heart.png", WidthRequest = 24, HeightRequest = 24 };
        heartIcon.SetBinding(Image.IsVisibleProperty, "IsFavorite");
        favoriteLabel.SetBinding(Label.TextProperty, "FavoriteStatusText");
        favoriteSwitch.SetBinding(MMC.Switch.IsToggledProperty, "IsFavorite");
        var favoriteRow = new HorizontalStackLayout
        {
            HorizontalOptions = LayoutOptions.Center,
            Spacing = 8,
            Children = { favoriteLabel, heartIcon, favoriteSwitch }
        };
        var card = new Border
        {
            BackgroundColor = (Application.Current?.Resources["White"] as Color) ?? Colors.White,
            StrokeThickness = 0,
            Padding = 20,
            Margin = new Thickness(0, 0, 0, 8),
            Content = new VerticalStackLayout
            {
                Spacing = 8,
                Children = { nameLabel, titleLabel, favoriteRow }
            }
        };

        // Properties CollectionView
        propertiesView = new CollectionView
        {
            IsGrouped = true,
            SelectionMode = SelectionMode.None,
            Margin = new Thickness(0, 8, 0, 0),
            GroupHeaderTemplate = new DataTemplate(() => {
                var label = new Label { FontAttributes = FontAttributes.Bold, FontSize = 16, Padding = new Thickness(8, 4, 0, 4) };
                label.SetBinding(Label.TextProperty, "Title");
                return label;
            }),
            ItemTemplate = new DataTemplate(() =>
            {
                var name = new Label { FontAttributes = FontAttributes.Bold };
                name.SetBinding(Label.TextProperty, "Name");
                var value = new Label();
                value.SetBinding(Label.TextProperty, "Value");
                return new Border
                {
                    BackgroundColor = (Application.Current?.Resources["Gray100"] as Color) ?? Colors.LightGray,
                    StrokeThickness = 0,
                    Padding = 12,
                    Margin = new Thickness(0, 2, 0, 2),
                    Content = new HorizontalStackLayout { Children = { name, value } }
                };
            })
        };
        propertiesView.SetBinding(ItemsView.ItemsSourceProperty, "PropertyGroups");

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Spacing = 24,
                Padding = 24,
                Children = { photoBorder, card, propertiesView }
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        favoriteSwitch.Toggled += OnFavoriteClicked;
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        if (BindingContext is PersonViewModel personInfo)
        {
            Title = personInfo.Person.Name;
            favoriteSwitch.IsToggled = personInfo.IsFavorite;
            DownloadImage();
        }
    }

    private void OnFavoriteClicked(object? sender, ToggledEventArgs e)
    {
        if (BindingContext is PersonViewModel personInfo)
        {
            personInfo.ToggleFavorite();
            Navigation.PopAsync();
        }
    }

    private void DownloadImage()
    {
        if (BindingContext is PersonViewModel personInfo)
        {
            var person = personInfo.Person;
            if (person.HasEmail)
            {
                var imageUrl = Gravatar.GetImageUrl(person.Email, IMAGE_SIZE);
                photo.Source = new UriImageSource { Uri = imageUrl };
            }
        }
    }
}
