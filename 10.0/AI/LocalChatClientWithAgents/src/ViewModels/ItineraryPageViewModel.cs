using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using LocalChatClientWithAgents.Models;
using LocalChatClientWithAgents.Services;

namespace LocalChatClientWithAgents.ViewModels;

[QueryProperty(nameof(Landmark), "Landmark")]
[QueryProperty(nameof(DayCount), "DayCount")]
[QueryProperty(nameof(Language), "Language")]
public partial class ItineraryPageViewModel(ItineraryService itineraryService, WeatherService weatherService, IDispatcher dispatcher) : ObservableObject
{
	public enum GenerationState
	{
		Generating,
		Complete,
		Error
	}

	private CancellationTokenSource _cancellationTokenSource = new();

	[ObservableProperty]
	public partial Landmark Landmark { get; set; }

	[ObservableProperty]
	public partial int DayCount { get; set; } = 3;

	[ObservableProperty]
	public partial string Language { get; set; } = "English";

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsGeneratingState))]
	[NotifyPropertyChangedFor(nameof(HasItinerary))]
	public partial ItineraryViewModel? Itinerary { get; set; }

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsGeneratingState))]
	[NotifyPropertyChangedFor(nameof(HasItinerary))]
	[NotifyPropertyChangedFor(nameof(IsErrorState))]
	[NotifyPropertyChangedFor(nameof(IsNotErrorState))]
	public partial GenerationState CurrentState { get; set; } = GenerationState.Generating;

	[ObservableProperty]
	public partial string? ErrorMessage { get; set; }

	/// <summary>Oldest status message (most faded, opacity 0.3).</summary>
	[ObservableProperty]
	public partial string? Status1 { get; set; }

	/// <summary>Middle status message (opacity 0.6).</summary>
	[ObservableProperty]
	public partial string? Status2 { get; set; }

	/// <summary>Newest status message (solid, opacity 1.0).</summary>
	[ObservableProperty]
	public partial string? Status3 { get; set; }

	public bool IsGeneratingState => CurrentState == GenerationState.Generating && Itinerary is null;
	public bool HasItinerary => CurrentState == GenerationState.Complete || Itinerary is not null;
	public bool IsErrorState => CurrentState == GenerationState.Error;
	public bool IsNotErrorState => CurrentState != GenerationState.Error;

	public async Task GenerateAsync()
	{
		_cancellationTokenSource.Cancel();
		_cancellationTokenSource.Dispose();
		_cancellationTokenSource = new CancellationTokenSource();
		var cancellationToken = _cancellationTokenSource.Token;

		CurrentState = GenerationState.Generating;
		ErrorMessage = string.Empty;
		Itinerary = null;
		Status1 = null;
		Status2 = null;
		Status3 = null;

		try
		{
			await Task.Run(() => BuildItineraryAsync(cancellationToken), cancellationToken);

			// Fetch weather for each day
			if (Itinerary is not null && !cancellationToken.IsCancellationRequested)
			{
				foreach (var dayVm in Itinerary.Days)
				{
					if (cancellationToken.IsCancellationRequested)
						break;

					dayVm.WeatherForecast = await weatherService.GetWeatherForecastAsync(
						Landmark.Latitude,
						Landmark.Longitude,
						dayVm.Date);
				}
			}

			if (!cancellationToken.IsCancellationRequested)
			{
				CurrentState = GenerationState.Complete;
			}
		}
		catch (OperationCanceledException)
		{
			// Ignore
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Itinerary generation failed: {ex}");
			CurrentState = GenerationState.Error;
			ErrorMessage = ex.Message;
		}
	}

	public void Cancel()
	{
		_cancellationTokenSource.Cancel();
		_cancellationTokenSource.Dispose();
	}

	private async Task BuildItineraryAsync(CancellationToken cancellationToken)
	{
		await foreach (var update in itineraryService.StreamItineraryAsync(Landmark, DayCount, Language, cancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
				break;

			if (update.StatusMessage is not null)
			{
				dispatcher.Dispatch(() =>
				{
					if (cancellationToken.IsCancellationRequested)
						return;

					Status1 = Status2;
					Status2 = Status3;
					Status3 = update.StatusMessage;
				});
			}

			if (update.PartialItinerary is not null)
			{
				dispatcher.Dispatch(() =>
				{
					if (cancellationToken.IsCancellationRequested)
						return;

					Itinerary = new ItineraryViewModel(update.PartialItinerary, Landmark);
				});
			}
		}
	}
}
