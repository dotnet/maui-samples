using ControlGallery.Models;

namespace ControlGallery.Views.XAML;

public partial class CarouselViewDemoPage : ContentPage
{
    public IReadOnlyList<Animal> Animals { get; } = AnimalData.All;

    public CarouselViewDemoPage()
    {
        InitializeComponent();
        BindingContext = this;
    }
}
