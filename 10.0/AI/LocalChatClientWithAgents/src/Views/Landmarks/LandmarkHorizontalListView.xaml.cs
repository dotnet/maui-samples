using System.Collections;
using LocalChatClientWithAgents.Models;

namespace LocalChatClientWithAgents.Views.Landmarks;

public partial class LandmarkHorizontalListView : ContentView
{
	public static readonly BindableProperty LandmarksProperty =
		BindableProperty.Create(nameof(Landmarks), typeof(IEnumerable), typeof(LandmarkHorizontalListView), null);

	public IEnumerable? Landmarks
	{
		get => (IEnumerable?)GetValue(LandmarksProperty);
		set => SetValue(LandmarksProperty, value);
	}

	public event EventHandler<Landmark>? LandmarkTapped;

	public LandmarkHorizontalListView()
	{
		InitializeComponent();
	}

	private void OnLandmarkItemTapped(object? sender, Landmark landmark)
	{
		LandmarkTapped?.Invoke(this, landmark);
	}
}
