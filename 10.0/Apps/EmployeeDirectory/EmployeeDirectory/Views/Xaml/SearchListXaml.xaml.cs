using EmployeeDirectory.Core.Data;
using EmployeeDirectory.Core.ViewModels;
using EmployeeDirectory.Core.Services;

namespace EmployeeDirectory.Views.Xaml;

public partial class SearchListXaml : ContentPage
{
	private Search search;
	private SearchViewModel viewModel;
	private IFavoritesRepository favoritesRepository;

	public SearchListXaml ()
	{
		InitializeComponent ();

		// Initialize favorites repository synchronously
		favoritesRepository = XmlFavoritesRepository.OpenFile ("XamarinFavorites.json").GetAwaiter().GetResult();

		search = new Search ("test");
		viewModel = new SearchViewModel (App.Service, search);

		viewModel.SearchCompleted += (sender, e) => {
			collectionView.ItemsSource = viewModel.Groups ?? new ObservableCollection<PeopleGroup>();
		};

		viewModel.Error += async (sender, e) => {
			await DisplayAlertAsync ("Error", e.Exception.Message, "OK");
		};

		BindingContext = viewModel;
	}

	private void OnValueChanged (object sender, TextChangedEventArgs e)
	{
		viewModel.Search ();
	}

	private async void OnItemSelected (object sender, SelectionChangedEventArgs e)
	{
		if (e.CurrentSelection.FirstOrDefault() is Person personInfo) {
			var employeeView = new EmployeeXaml {
				BindingContext = new PersonViewModel (personInfo, favoritesRepository)
			};
			await Navigation.PushAsync (employeeView);
			collectionView.SelectedItem = null;
		}
	}
}