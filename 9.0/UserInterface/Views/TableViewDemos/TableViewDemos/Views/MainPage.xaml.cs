using System.Windows.Input;
using System.Collections.ObjectModel;

namespace TableViewDemos;

public partial class MainPage : ContentPage
{
	public ICommand NavigateCommand { get; private set; }
	public ObservableCollection<MenuItem> MenuItems { get; } = new ObservableCollection<MenuItem>();

	public MainPage()
	{
		InitializeComponent();

		NavigateCommand = new Command<Type>(
			async (Type pageType) =>
			{
				Page page = (Page)Activator.CreateInstance(pageType);
				await Navigation.PushAsync(page);
			});
			
		// Populate menu items
		MenuItems.Add(new MenuItem { Title = "Menu page", PageType = typeof(MenuIntentPage) });
		MenuItems.Add(new MenuItem { Title = "Settings page", PageType = typeof(SettingsIntentPage) });
		MenuItems.Add(new MenuItem { Title = "Form page", PageType = typeof(FormIntentPage) });
		MenuItems.Add(new MenuItem { Title = "Data page", PageType = typeof(DataIntentPage) });
		MenuItems.Add(new MenuItem { Title = "ImageCell page", PageType = typeof(ImageCellPage) });
		MenuItems.Add(new MenuItem { Title = "Right to left TableView", PageType = typeof(RightToLeftTablePage) });
		
		BindingContext = this;
	}
}

// Data model for menu items
public class MenuItem
{
	public string Title { get; set; }
	public Type PageType { get; set; }
}
