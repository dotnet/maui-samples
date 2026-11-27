using System.Collections.ObjectModel;
using System.Windows.Input;
using ControlGallery.Models;

namespace ControlGallery.Views.XAML;

public partial class SwipeViewDemoPage : ContentPage
{
    public ObservableCollection<Animal> Animals { get; } = new(AnimalData.All);

    public ICommand DeleteCommand { get; }

    public SwipeViewDemoPage()
    {
        InitializeComponent();
        DeleteCommand = new Command<Animal>(animal => Animals.Remove(animal));
        BindingContext = this;
    }
}
