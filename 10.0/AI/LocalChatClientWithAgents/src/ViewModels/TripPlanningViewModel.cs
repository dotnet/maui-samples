using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalChatClientWithAgents.Models;
using LocalChatClientWithAgents.Services;

namespace LocalChatClientWithAgents.ViewModels;

[QueryProperty(nameof(Landmark), "Landmark")]
public partial class TripPlanningViewModel(TaggingService taggingService) : ObservableObject
{
	private CancellationTokenSource _cancellationTokenSource = new();

	[ObservableProperty]
	public partial Landmark Landmark { get; set; }

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(IncrementDaysCommand))]
	[NotifyCanExecuteChangedFor(nameof(DecrementDaysCommand))]
	public partial int DayCount { get; set; } = 3;

	public ObservableCollection<string> GeneratedTags => field ??= [];

	[RelayCommand(CanExecute = nameof(CanIncrementDays))]
	void IncrementDays() => DayCount++;

	[RelayCommand(CanExecute = nameof(CanDecrementDays))]
	void DecrementDays() => DayCount--;

	bool CanIncrementDays() => DayCount < 7;
	bool CanDecrementDays() => DayCount > 1;

	public async Task InitializeAsync()
	{
		if (Landmark is null || GeneratedTags.Count > 0)
			return;

		await GenerateTagsAsync(_cancellationTokenSource.Token);
	}

	public void Cancel()
	{
		_cancellationTokenSource.Cancel();
	}

	private async Task GenerateTagsAsync(CancellationToken cancellationToken)
	{
		try
		{
			var tags = await taggingService.GenerateTagsAsync(Landmark.Description, cancellationToken);
			GeneratedTags.Clear();
			foreach (var tag in tags)
			{
				if (cancellationToken.IsCancellationRequested)
					break;
				GeneratedTags.Add(tag);
				await Task.Delay(100, cancellationToken);
			}
		}
		catch (OperationCanceledException)
		{
			// Ignore for cancellation
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Tag generation failed: {ex.Message}");
		}
	}
}
