namespace WorkingWithFonts;

public class FontPageCs : ContentPage
{
    public FontPageCs()
    {
        var label = new Label
        {
            Text = "Hello, .NET MAUI!",
            FontFamily = "Lobster"
        };

        // Platform-specific font size
        var platform = DeviceInfo.Platform;
        if (platform == DevicePlatform.iOS)
            label.FontSize = 24;
        else if (platform == DevicePlatform.Android)
            label.FontSize = (double)18;
        else
            label.FontSize = 32;

        var labelBold = new Label
        {
            Text = "Bold",
            FontSize = 14,
            FontAttributes = FontAttributes.Bold
        };
        var labelItalic = new Label
        {
            Text = "Italic",
            FontSize = 14,
            FontAttributes = FontAttributes.Italic
        };
        var labelBoldItalic = new Label
        {
            Text = "BoldItalic",
            FontSize = 14,
            FontAttributes = FontAttributes.Bold | FontAttributes.Italic
        };

        // Span formatting support
        var labelFormatted = new Label();
        var fs = new FormattedString();
        fs.Spans.Add(new Span { Text = "Red, ", TextColor = Colors.Red, FontSize = 20, FontAttributes = FontAttributes.Italic });
        fs.Spans.Add(new Span { Text = " blue, ", TextColor = Colors.Blue, FontSize = 32 });
        fs.Spans.Add(new Span { Text = " and green!", TextColor = Colors.Green, FontSize = 12 });
        labelFormatted.FormattedText = fs;

        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };

        grid.Add(label, 0, 0);
        grid.Add(labelBold, 0, 1);
        grid.Add(labelItalic, 0, 2);
        grid.Add(labelBoldItalic, 0, 3);
        grid.Add(labelFormatted, 0, 4);

        Content = grid;
    }
}

