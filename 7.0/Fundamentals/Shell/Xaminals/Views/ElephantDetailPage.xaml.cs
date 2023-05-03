using Xaminals.Models;

namespace Xaminals.Views
{
    [QueryProperty(nameof(Name), "name")]
    [QueryProperty(nameof(Elephant), "Elephant")]
    public partial class ElephantDetailPage : ContentPage
    {
        public string Name { get; set; }

        Animal elephant;
        public Animal Elephant
        {
            get => elephant;
            set
            {
                elephant = value;
                OnPropertyChanged();
            }
        }

        public ElephantDetailPage()
        {
            InitializeComponent();
            BindingContext = this;
        }
    }
}
