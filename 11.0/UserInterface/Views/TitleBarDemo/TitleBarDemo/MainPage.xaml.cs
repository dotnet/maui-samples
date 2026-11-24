using Microsoft.Maui.Controls.Shapes;

namespace TitleBarDemo
{
    public partial class MainPage : ContentPage
    {
        TitleBar? _customTitleBar;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _customTitleBar = Window.TitleBar as TitleBar;
        }

        private void SetIconCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                _customTitleBar!.Icon = "tb_appicon.png";
            }
            else
            {
                _customTitleBar!.Icon = string.Empty;
            }
        }

        private void ColorButton_Clicked(object sender, EventArgs e)
        {
            if (Color.TryParse(ColorTextBox.Text, out var color))
            {
                _customTitleBar!.BackgroundColor = color;
            }
        }

        private void ForegroundColorButton_Clicked(object sender, EventArgs e)
        {
            if (Color.TryParse(ForegroundColorTextBox.Text, out var color))
            {
                _customTitleBar!.ForegroundColor = color;
            }
        }

        private void LeadingCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                _customTitleBar!.LeadingContent = new Button()
                {
                    Text = "Leading"
                };
            }
            else
            {
                _customTitleBar!.LeadingContent = null;
            }
        }

        private void ContentCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                _customTitleBar!.Content = new SearchBar()
                {
                    Placeholder = "Search",
                    MaximumWidthRequest = 300,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Center
                };
            }
            else
            {
                _customTitleBar!.Content = null;
            }
        }

        private void TrailingCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                _customTitleBar!.TrailingContent = new Border()
                {
                    WidthRequest = 32,
                    HeightRequest = 32,
                    StrokeShape = new Ellipse() { WidthRequest = 32, HeightRequest = 32 },
                    StrokeThickness = 0,
                    BackgroundColor = Colors.Azure,
                    Content = new Label()
                    {
                        Text = "User",
                        TextColor = Colors.Black,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        FontSize = 10
                    }
                };
            }
            else
            {
                _customTitleBar!.TrailingContent = null;
            }
        }

        private void TallModeCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                _customTitleBar!.HeightRequest = 48;
            }
            else
            {
                _customTitleBar!.HeightRequest = 32;
            }
        }
    }
}
