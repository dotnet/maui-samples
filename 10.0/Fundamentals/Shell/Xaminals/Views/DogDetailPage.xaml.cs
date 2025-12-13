using Xaminals.Data;
using Xaminals.Models;

namespace Xaminals.Views
{
    [QueryProperty(nameof(Name), "name")]
    public partial class DogDetailPage : ContentPage
    {
        public string Name
        {
            set
            {
                LoadAnimal(value);
            }
        }

        public DogDetailPage()
        {
            InitializeComponent();
        }

        void LoadAnimal(string name)
        {
            try
            {
                Animal animal = DogData.Dogs.FirstOrDefault(a => a.Name == name);
                BindingContext = animal;
            }
            catch (ArgumentNullException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Argument null error loading dog: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Invalid operation loading dog: {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load dog: {ex.Message}");
            }
        }
    }
}
