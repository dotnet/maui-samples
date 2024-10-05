namespace SwitchDemos
{
    public class SwitchCodePage : ContentPage
    {
        Label switchStateLabel;

        public SwitchCodePage()
        {
            Title = "Switch Code Demo";

            switchStateLabel = new Label
            {
                Text = "Switch state is OFF",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            Switch switchControl = new Switch
            {
                IsToggled = false,
                OnColor = Colors.Orange,
                ThumbColor = Colors.Green,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            switchControl.Toggled += SwitchControl_Toggled;

            Content = new StackLayout
            {
                Children =
                {
                    switchStateLabel,
                    switchControl
                }
            };
        }

        void SwitchControl_Toggled(object sender, ToggledEventArgs e)
        {
            string stateName = e.Value ? "ON" : "OFF";
            switchStateLabel.Text = $"The switch is {stateName}";
        }
    }
}
