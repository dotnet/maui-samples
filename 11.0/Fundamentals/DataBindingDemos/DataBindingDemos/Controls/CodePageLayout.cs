namespace DataBindingDemos.Controls;

// Builds the layout that the XAML pages declare in markup, for the pages written in C#:
// a header (eyebrow, title, and lead) above a card that holds the demo.
public static class CodePageLayout
{
    public static View Create(string eyebrow, string title, string lead, View demo)
    {
        var titleLabel = new Label { Text = title, Style = GetStyle("Title1") };
        SemanticProperties.SetHeadingLevel(titleLabel, SemanticHeadingLevel.Level1);

        var header = new VerticalStackLayout
        {
            Style = GetStyle("DemoHeader"),
            Children =
            {
                new Label { Text = eyebrow, Style = GetStyle("Eyebrow") },
                titleLabel,
                new Label { Text = lead, Style = GetStyle("Lead") }
            }
        };

        var layout = new Grid
        {
            Style = GetStyle("DemoLayout"),
            RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Auto) }
        };
        layout.Add(header);
        layout.Add(new Border { Style = GetStyle("DemoSurface"), Content = demo }, 0, 1);

        return new ScrollView { Content = layout };
    }

    public static Style GetStyle(string key) =>
        Application.Current!.Resources.TryGetValue(key, out var style) ? (Style)style : null;
}
