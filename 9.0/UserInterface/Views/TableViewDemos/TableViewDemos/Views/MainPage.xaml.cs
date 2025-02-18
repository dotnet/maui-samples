using System.Windows.Input;

namespace TableViewDemos;

public class MenuItem
{
	public string Text { get; set; }
	public Type PageType { get; set; }
}

public partial class MainPage : ContentPage
{
	public ICommand NavigateCommand { get; private set; }

	public MenuItem[] Pages { get; private set; }

	public MainPage()
	{
		Pages =
		[
			new MenuItem { Text = "Menu page", PageType = typeof(MenuIntentPage) },
			new MenuItem { Text = "Settings page", PageType = typeof(SettingsIntentPage) },
			new MenuItem { Text = "Form page", PageType = typeof(FormIntentPage) },
			new MenuItem { Text = "Data page", PageType = typeof(DataIntentPage) },
			new MenuItem { Text = "ImageCell page", PageType = typeof(ImageCellPage) },
			new MenuItem { Text = "Right to left TableView", PageType = typeof(RightToLeftTablePage) },
		];
		BindingContext = this;

		InitializeComponent();

		NavigateCommand = new Command<Type>(
			async (Type pageType) =>
			{
				Page page = (Page)Activator.CreateInstance(pageType);
				await Navigation.PushAsync(page);
			});
	}
}
