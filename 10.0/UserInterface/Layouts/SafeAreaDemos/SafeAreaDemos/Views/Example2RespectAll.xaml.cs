using SafeAreaDemos.ViewModels;

namespace SafeAreaDemos.Views;

public partial class Example2RespectAll : ContentPage
{
	private Example2RespectAllViewModel viewModel;

	public Example2RespectAll()
	{
		InitializeComponent();

		viewModel = new Example2RespectAllViewModel();
		BindingContext = viewModel;
	}

	void OnSearchTextChanged(object sender, TextChangedEventArgs e)
	{
		viewModel.FilterContacts(e.NewTextValue);
	}

	void OnContactListTapped(object sender, EventArgs e)
	{
		// Dismiss the keyboard when tapping on the contact list
		contactSearchBar.Unfocus();
	}

	async void Button_Clicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
}