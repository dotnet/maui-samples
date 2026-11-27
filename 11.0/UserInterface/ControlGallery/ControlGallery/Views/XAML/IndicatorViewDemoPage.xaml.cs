using ControlGallery.Models;

namespace ControlGallery.Views.XAML;

public partial class IndicatorViewDemoPage : ContentPage
{
    public IReadOnlyList<Animal> Animals { get; } = AnimalData.All;

    public IndicatorViewDemoPage()
    {
        InitializeComponent();
        BindingContext = this;
    }
}
