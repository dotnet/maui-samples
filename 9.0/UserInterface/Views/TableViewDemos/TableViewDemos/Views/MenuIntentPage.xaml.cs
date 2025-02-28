using System.Collections.ObjectModel;
using System.Windows.Input;
using TableViewDemos.Models;
using CustomMenuItem = TableViewDemos.Models.MenuListItem;

namespace TableViewDemos
{
	public partial class MenuIntentPage : ContentPage
	{
		public ObservableCollection<MenuSection> Sections { get; } = new();

		public MenuIntentPage()
		{
			InitializeComponent();
			BindingContext = this;

			// Initialize menu sections and items
			Sections.Add(new MenuSection
			{
				Title = "Chapters",
				Items = new List<CustomMenuItem>
				{
					new CustomMenuItem 
					{ 
						Text = "1. Introduction to .NET MAUI",
						Detail = "Learn about .NET MAUI and what it provides.",
						Command = new Command(() => OnChapterSelected("Introduction to .NET MAUI"))
					},
					new CustomMenuItem 
					{ 
						Text = "2. Anatomy of an app",
						Detail = "Learn about the visual elements in .NET MAUI",
						Command = new Command(() => OnChapterSelected("Anatomy of an app"))
					},
					new CustomMenuItem 
					{ 
						Text = "3. Text",
						Detail = "Learn about the .NET MAUI controls that display text.",
						Command = new Command(() => OnChapterSelected("Text"))
					},
					new CustomMenuItem 
					{ 
						Text = "4. Dealing with sizes",
						Detail = "Learn how to size .NET MAUI controls on screen.",
						Command = new Command(() => OnChapterSelected("Dealing with sizes"))
					},
					new CustomMenuItem 
					{ 
						Text = "5. XAML vs code",
						Detail = "Learn more about creating your UI in XAML.",
						Command = new Command(() => OnChapterSelected("XAML vs code"))
					}
				}
			});
		}

		private async void OnChapterSelected(string chapter)
		{
			await DisplayAlert("Chapter Selected", $"You selected: {chapter}", "OK");
		}
	}
}

