namespace WordPuzzle
{
    class GameSquare : ContentView
    {
        Label label;
        string normText, winText;

        // Retain current Row and Col position.
        public int Index { get; private set; }
        public int Row { get; set; }
        public int Col { get; set; }

        public GameSquare(char normChar, char winChar, int index)
        {
            this.Index = index;
            this.normText = normChar.ToString();
            this.winText = winChar.ToString();

            // A Frame surrounding two Labels.
            label = new Label
            {
                Text = this.normText,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            Label tinyLabel = new Label
            {
                Text = (index + 1).ToString(),
                FontSize = (DeviceInfo.Current.Idiom == DeviceIdiom.Desktop ? 10 : 14),
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.End
            };

            this.Padding = new Thickness(5);
            this.Content = new Frame
            {
                BorderColor = Colors.Black,
                HasShadow = true,
                Padding = new Thickness(5, 10, 5, 0),
                Content = new VerticalStackLayout
                {
                    label, tinyLabel,
                }
            };

            // Don't let touch pass us by.
            this.BackgroundColor = Colors.Transparent;
        }

        public async Task AnimateWinAsync(bool isReverse)
        {
            uint length = 150;
            await Task.WhenAll(this.ScaleTo(3, length), this.RotateTo(180, length));
            label.Text = isReverse ? normText : winText;
            await Task.WhenAll(this.ScaleTo(1, length), this.RotateTo(360, length));
            this.Rotation = 0;
        }

        public void SetLabelFont(double fontSize, FontAttributes attributes)
        {
            label.FontSize = fontSize;
            label.FontAttributes = attributes;
        }
    }
}
