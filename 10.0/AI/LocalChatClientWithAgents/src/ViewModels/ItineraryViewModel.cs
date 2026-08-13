using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LocalChatClientWithAgents.Models;

namespace LocalChatClientWithAgents.ViewModels;

public partial class ItineraryViewModel(Itinerary itinerary, Landmark landmark) : ObservableObject
{
	public string Title => itinerary.Title;

	public string Description => itinerary.Description;

	public string Rationale => itinerary.Rationale;

	public ObservableCollection<DayPlanViewModel> Days =>
		field ??= CreateDays();

	private ObservableCollection<DayPlanViewModel> CreateDays()
	{
		if (itinerary.Days is null or { Count: 0 })
			return [];

		var startDate = DateOnly.FromDateTime(DateTime.Today);

		var list = new ObservableCollection<DayPlanViewModel>();
		for (int i = 0; i < itinerary.Days.Count; i++)
		{
			list.Add(new DayPlanViewModel(itinerary.Days[i], landmark, startDate.AddDays(i)));
		}

		return list;
	}
}
