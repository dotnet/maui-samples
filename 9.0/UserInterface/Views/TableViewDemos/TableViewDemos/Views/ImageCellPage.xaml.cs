using System.Collections.ObjectModel;
using System.Windows.Input;

namespace TableViewDemos;

public partial class ImageCellPage : ContentPage
{
	public ObservableCollection<Models.MenuSection> Sections { get; } = new();

	public ImageCellPage()
	{
		InitializeComponent();
		BindingContext = this;

		// Initialize sections and items
		Sections.Add(new Models.MenuSection
		{
			Title = "Learn how to use your Xbox",
			Items = new List<Models.MenuListItem>
			{
				new Models.MenuListItem 
				{ 
					Text = "1. Introduction",
					Detail = "Learn about your Xbox and its capabilities.",
					ImageSource = "xbox.png",
					Command = new Command(() => OnTopicSelected("Introduction"))
				},
				new Models.MenuListItem 
				{ 
					Text = "2. Turn it on",
					Detail = "Learn how to turn on your Xbox.",
					ImageSource = "xbox.png",
					Command = new Command(() => OnTopicSelected("Turn it on"))
				},
				new Models.MenuListItem 
				{ 
					Text = "3. Connect your controller",
					Detail = "Learn how to connect your wireless controller.",
					ImageSource = "xbox.png",
					Command = new Command(() => OnTopicSelected("Connect your controller"))
				},
				new Models.MenuListItem 
				{ 
					Text = "4. Launch a game",
					Detail = "Learn how to launch a game.",
					ImageSource = "xbox.png",
					Command = new Command(() => OnTopicSelected("Launch a game"))
				}
			}
		});
	}

	private async void OnTopicSelected(string topic)
	{
		await DisplayAlert("Topic Selected", $"You selected: {topic}", "OK");
	}
}