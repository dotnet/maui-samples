using Xaminals.Models;

namespace Xaminals.Views
{
    public partial class MonkeysPage : ContentPage
    {
        public MonkeysPage()
        {
            InitializeComponent();
        }

        async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Animal animal = e.CurrentSelection.FirstOrDefault() as Animal;
            var navigationParameters = new Dictionary<string, object>
            {
                { "Monkey", animal }
            };
            await Shell.Current.GoToAsync($"monkeydetails", navigationParameters);
        }
    }
}
