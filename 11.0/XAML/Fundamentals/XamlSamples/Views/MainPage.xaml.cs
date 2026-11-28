using System.Windows.Input;

namespace XamlSamples;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();

		NavigateCommand = new Command<PageDataViewModel>(async pageData =>
		{
			Page page = (Page)Activator.CreateInstance(pageData.Type);
			await Navigation.PushAsync(page);
		});

		BindingContext = this;
	}

	public IReadOnlyList<PageSection> Sections => PageSection.All;

	public ICommand NavigateCommand { get; }

	protected override void OnSizeAllocated(double width, double height)
	{
		base.OnSizeAllocated(width, height);

		// Give the list more room on short screens, such as a phone in landscape.
		hero.IsVisible = height >= 560;
	}
}
