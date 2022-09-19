namespace BehaviorsDemos
{
    public class AttachedNumericValidationPageCode : ContentPage
    {
        public AttachedNumericValidationPageCode()
        {
            Title = "Numeric";
            IconImageSource = "csharp.png";

            var entry = new Entry { Placeholder = "Enter a System.Double" };
            AttachedNumericValidationBehavior.SetAttachBehavior(entry, true);

            Content = new StackLayout
            {
                Padding = new Thickness(0, 20, 0, 0),
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
