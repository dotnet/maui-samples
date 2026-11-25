namespace GridDemos.Views.XAML
{
    public partial class ColorSlidersGridPage : ContentPage
    {
        public ColorSlidersGridPage()
        {
            InitializeComponent();
        }

        void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
		{
            boxView.Color = new Color((float)redSlider.Value, (float)greenSlider.Value, (float)blueSlider.Value);
		}
    }
}
