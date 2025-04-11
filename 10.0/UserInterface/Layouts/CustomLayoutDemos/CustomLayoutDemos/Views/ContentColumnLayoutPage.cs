using CustomLayoutDemos.Layouts;

namespace CustomLayoutDemos.Views;

public class ContentColumnLayoutPage : ContentPage
{
	public ContentColumnLayoutPage()
	{
		Title = "Content column layout";

		var header = new Label
		{
			Text = "This is the column header",
			Margin = new Thickness(10),
			FontAttributes = FontAttributes.Bold
		};

		var content = new Label
		{
            Text = "This is the content view. This layout is pre-made to have the 'Content' view fill up the remaining vertical space " +
                "after the header and footer have been laid out. The layout is a subclass of Layout which implements IGridLayout; it takes " +
                "a header, content, and footer view in its constuctor and lays them out in a single column. No custom layout manager is required; " +
                "it simply uses the built-in GridLayoutManager.",
            LineBreakMode = LineBreakMode.WordWrap,
            BackgroundColor = Colors.LightBlue,
            Padding = new Thickness(10)
        };

		var footer = new Label
		{
			Text = "This is the footer",
            Margin = new Thickness(10),
            FontAttributes = FontAttributes.Italic
		};

		Content = new ContentColumnLayout(header, content, footer);
	}
}
