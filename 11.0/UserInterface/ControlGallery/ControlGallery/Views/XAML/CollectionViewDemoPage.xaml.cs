using ControlGallery.Models;

namespace ControlGallery.Views.XAML;

public partial class CollectionViewDemoPage : ContentPage
{
    public IReadOnlyList<Animal> Animals { get; } = AnimalData.All;

    public CollectionViewDemoPage()
    {
        InitializeComponent();
        BindingContext = this;
    }
}
