using LocalChatClientWithAgents.Models;
using LocalChatClientWithAgents.ViewModels;

namespace LocalChatClientWithAgents.Pages;

public partial class TripPlanningPage : ContentPage
{
	private readonly TripPlanningViewModel _viewModel;

	public TripPlanningPage(TripPlanningViewModel viewModel)
	{
		InitializeComponent();

		_viewModel = viewModel;
		BindingContext = viewModel;

		Loaded += async (_, _) => await viewModel.InitializeAsync();
		NavigatingFrom += (_, _) => viewModel.Cancel();
	}

	private async void OnGenerateClicked(object? sender, EventArgs e)
	{
		var parameters = new Dictionary<string, object>
		{
			{ "Landmark", _viewModel.Landmark },
			{ "DayCount", _viewModel.DayCount }
		};
		await Shell.Current.GoToAsync($"../{nameof(ItineraryPage)}", parameters);
	}

	private async void OnBackButtonClicked(object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("..");
	}
}
