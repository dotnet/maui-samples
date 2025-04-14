﻿using CardViewDemo.Controls;

namespace CardViewDemo
{
    public class CardViewCodePage : ContentPage
    {
        public CardViewCodePage()
        {
            Title = "CardView Code Demo";
            Padding = 10;

            StackLayout layout = new StackLayout
            {
                Spacing = 6,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Children =
                {
                    new CardView
                    {
                        BorderColor = Colors.DarkGray,
                        CardTitle = "Slavko Vlasic",
                        CardDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla elit dolor, convallis non interdum.",
                        IconBackgroundColor = Colors.SlateGray,
                        IconImageSource = ImageSource.FromFile("user.png")
                    },
                    new CardView
                    {
                        BorderColor = Colors.DarkGray,
                        CardTitle = "Carolina Pena",
                        CardDescription = "Phasellus eu convallis mi. In tempus augue eu dignissim fermentum. Morbi ut lacus vitae eros lacinia.",
                        IconBackgroundColor = Colors.SlateGray,
                        IconImageSource = ImageSource.FromFile("user.png")
                    },
                    new CardView
                    {
                        BorderColor = Colors.DarkGray,
                        CardTitle = "Wade Blanks",
                        CardDescription = "Aliquam sagittis, odio lacinia fermentum dictum, mi erat scelerisque erat, quis aliquet arcu.",
                        IconBackgroundColor = Colors.SlateGray,
                        IconImageSource = ImageSource.FromFile("user.png")
                    },
                    new CardView
                    {
                        BorderColor = Colors.DarkGray,
                        CardTitle = "Colette Quint",
                        CardDescription = "In pellentesque odio eget augue elementum lobortis. Sed augue massa, rhoncus eu nisi vitae, egestas.",
                        IconBackgroundColor = Colors.SlateGray,
                        IconImageSource = ImageSource.FromFile("user.png")
                    },
                }
            };

            ScrollView scroll = new ScrollView
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Content = layout
            };

            Content = scroll;
        }
    }
}