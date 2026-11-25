namespace BehaviorsDemos
{
    public class NumericValidationStylePageCode : ContentPage
    {
        public NumericValidationStylePageCode()
        {
            Title = "Numeric";
            IconImageSource = "csharp.png";

            var entry = new Entry { Placeholder = "Enter a System.Double" };
            NumericValidationStyleBehavior.SetAttachBehavior(entry, true);

            Content = new StackLayout
            {
                Padding = new Thickness(5, 50, 5, 0),
                Children = {
                    new Label {
                        Text = "Red when the number isn't valid",
                        FontSize = 12
                    },
                    entry
                }
            };
        }
    }
}
