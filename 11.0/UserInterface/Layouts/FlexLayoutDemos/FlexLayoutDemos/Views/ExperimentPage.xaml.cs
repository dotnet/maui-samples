namespace FlexLayoutDemos.Views
{
    public partial class ExperimentPage : ContentPage
    {
        static Color[] colors = { Colors.Red, Colors.Magenta, Colors.Blue,
                                  Colors.Cyan, Colors.Green, Colors.Yellow };

        static string[] digitsText = { "", "One", "Two", "Three", "Four", "Five",
                                       "Six", "Seven", "Eight", "Nine", "Ten",
                                       "Eleven", "Twelve", "Thirteen", "Fourteen",
                                       "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };

        static string[] decadeText = { "", "", "Twenty", "Thirty", "Forty", "Fifty",
                                       "Sixty", "Seventy", "Eighty", "Ninety" };

        public ExperimentPage()
        {
            InitializeComponent();

            OnNumberStepperValueChanged(flexLayout, new ValueChangedEventArgs(0, numberStepper.Value));
        }

        void OnNumberStepperValueChanged(object sender, ValueChangedEventArgs args)
        {
            int count = (int)args.NewValue;

            while (flexLayout.Children.Count > count)
            {
                flexLayout.Children.RemoveAt(flexLayout.Children.Count - 1);
            }
            while (flexLayout.Children.Count < count)
            {
                int number = flexLayout.Children.Count + 1;
                string text = "";

                if (number < 20)
                {
                    text = digitsText[number];
                }
                else
                {
                    text = decadeText[number / 10] +
                           (number % 10 == 0 ? "" : "-") +
                                digitsText[number % 10];
                }

                Label label = new Label
                {
                    Text = text,
                    FontSize = 16 + 4 * ((number - 1) % 4),
                    TextColor = colors[(number - 1) % colors.Length],
                    BackgroundColor = Colors.LightGray
                };

                flexLayout.Children.Add(label);
            }
        }
    }
}