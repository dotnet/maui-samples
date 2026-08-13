using LocalChatClientWithAgents.ViewModels;

namespace LocalChatClientWithAgents.Pages;

public partial class ItineraryPage : ContentPage
{
	public ItineraryPage(ItineraryPageViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;

		Loaded += async (_, _) => await viewModel.GenerateAsync();
		NavigatingFrom += (_, _) => viewModel.Cancel();
	}

	private async void OnBackButtonClicked(object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("..");
	}
}
