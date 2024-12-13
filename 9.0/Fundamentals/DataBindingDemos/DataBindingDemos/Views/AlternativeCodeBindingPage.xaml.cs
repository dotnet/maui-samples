namespace DataBindingDemos
{
    public partial class AlternativeCodeBindingPage : ContentPage
    {
        public AlternativeCodeBindingPage()
        {
            InitializeComponent();

            label.SetBinding(Label.ScaleProperty, static (Slider s) => s.Value, source: slider);
        }
    }
}