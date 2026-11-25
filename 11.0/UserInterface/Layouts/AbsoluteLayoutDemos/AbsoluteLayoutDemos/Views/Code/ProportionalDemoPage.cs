using Microsoft.Maui.Layouts;

namespace AbsoluteLayoutDemos.Views.Code
{
    public class ProportionalDemoPage : ContentPage
    {
        public ProportionalDemoPage()
        {
            BoxView blue = new BoxView { Color = Colors.Blue };
            AbsoluteLayout.SetLayoutBounds(blue, new Rect(0.5, 0, 100, 25));
            AbsoluteLayout.SetLayoutFlags(blue, AbsoluteLayoutFlags.PositionProportional);

            BoxView green = new BoxView { Color = Colors.Green };
            AbsoluteLayout.SetLayoutBounds(green, new Rect(0, 0.5, 25, 100));
            AbsoluteLayout.SetLayoutFlags(green, AbsoluteLayoutFlags.PositionProportional);

            BoxView red = new BoxView { Color = Colors.Red };
            AbsoluteLayout.SetLayoutBounds(red, new Rect(1, 0.5, 25, 100));
            AbsoluteLayout.SetLayoutFlags(red, AbsoluteLayoutFlags.PositionProportional);

            BoxView black = new BoxView { Color = Colors.Black };
            AbsoluteLayout.SetLayoutBounds(black, new Rect(0.5, 1, 100, 25));
            AbsoluteLayout.SetLayoutFlags(black, AbsoluteLayoutFlags.PositionProportional);

            Label label = new Label { Text = "Centered text" };
            AbsoluteLayout.SetLayoutBounds(label, new Rect(0.5, 0.5, 110, 25));
            AbsoluteLayout.SetLayoutFlags(label, AbsoluteLayoutFlags.PositionProportional);

            Title = "Proportional demo";
            Content = new AbsoluteLayout
            {
                Children =
                {
                    blue,
                    green,
                    red,
                    black,
                    label
                }
            };
        }
    }
}

