using Microsoft.Maui.Controls;
using System;
using System.Globalization;

namespace TabbedPageDemo;

public class TabbedPageDemoPageCS : TabbedPage
{
    public TabbedPageDemoPageCS()
    {
        var booleanConverter = new NonNullToBooleanConverter();

        ItemTemplate = new DataTemplate(() =>
        {
            var nameLabel = new Label
            {
                FontSize = 24, // Large
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };
            nameLabel.SetBinding(Label.TextProperty, "Name");

            var image = new Image { WidthRequest = 200, HeightRequest = 200 };
            image.SetBinding(Image.SourceProperty, "PhotoUrl");

            var familyLabel = new Label
            {
                FontSize = 18, // Medium
                FontAttributes = FontAttributes.Bold
            };
            familyLabel.SetBinding(Label.TextProperty, "Family");

            var subFamilyLabel = new Label
            {
                FontSize = 18, // Medium
                FontAttributes = FontAttributes.Bold
            };
            subFamilyLabel.SetBinding(Label.TextProperty, "Subfamily");

            var tribeLabel = new Label
            {
                FontSize = 18, // Medium
                FontAttributes = FontAttributes.Bold
            };
            tribeLabel.SetBinding(Label.TextProperty, "Tribe");

            var genusLabel = new Label
            {
                FontSize = 18, // Medium
                FontAttributes = FontAttributes.Bold
            };
            genusLabel.SetBinding(Label.TextProperty, "Genus");

            var familyStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = {
                    new Label { Text = "Family:", HorizontalOptions = LayoutOptions.Fill },
                    familyLabel
                }
            };

            var subFamilyStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = {
                    new Label { Text = "Subfamily:", HorizontalOptions = LayoutOptions.Fill },
                    subFamilyLabel
                }
            };
            subFamilyStack.SetBinding(StackLayout.IsVisibleProperty, new Binding("Subfamily", converter: booleanConverter));

            var tribeStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = {
                    new Label { Text = "Tribe:", HorizontalOptions = LayoutOptions.Fill },
                    tribeLabel
                }
            };
            tribeStack.SetBinding(StackLayout.IsVisibleProperty, new Binding("Tribe", converter: booleanConverter));

            var genusStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = {
                    new Label { Text = "Genus:", HorizontalOptions = LayoutOptions.Fill },
                    genusLabel
                }
            };

            var infoStack = new StackLayout
            {
                Padding = new Thickness(50, 10),
                Children = { familyStack, subFamilyStack, tribeStack, genusStack }
            };

            var mainStack = new StackLayout
            {
                Padding = new Thickness(5, 25),
                Children = { nameLabel, image, infoStack }
            };

            var contentPage = new ContentPage
            {
                Content = mainStack
            };
            contentPage.SetBinding(ContentPage.TitleProperty, "Name");
            contentPage.SetBinding(ContentPage.IconImageSourceProperty, new Binding(".", converter: new MonkeyIconConverter()));

            return contentPage;
        });

        ItemsSource = MonkeyDataModel.All;
    }
}

public class MonkeyIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return "monkeyicon.png";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}