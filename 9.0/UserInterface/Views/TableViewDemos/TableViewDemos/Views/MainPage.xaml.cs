using System.Windows.Input;

namespace TableViewDemos;

public partial class MainPage : ContentPage
{
	public ICommand NavigateCommand { get; private set; }

	public object[] Pages { get; private set; }

	public MainPage()
	{
		Pages =
		[
			new { Text = "Menu page", PageType = typeof(MenuIntentPage) },
			new { Text = "Settings page", PageType = typeof(SettingsIntentPage) },
			new { Text = "Form page", PageType = typeof(FormIntentPage) },
			new { Text = "Data page", PageType = typeof(DataIntentPage) },
			new { Text = "ImageCell page", PageType = typeof(ImageCellPage) },
			new { Text = "Right to left TableView", PageType = typeof(RightToLeftTablePage) },
		];

		InitializeComponent();

		NavigateCommand = new Command<Type>(
			async (Type pageType) =>
			{
				Page page = (Page)Activator.CreateInstance(pageType);
				await Navigation.PushAsync(page);
			});
		BindingContext = this;
	}
}
