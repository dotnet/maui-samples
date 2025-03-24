using System.Windows.Input;

namespace ListViewDemos;

public partial class MainPage : ContentPage
{
	public ICommand NavigateCommand { get; private set; }

	public MainPage()
	{
		InitializeComponent();

		NavigateCommand = new Command<Type>(
			async (Type pageType) =>
			{
				Page page = (Page)Activator.CreateInstance(pageType);
				await Navigation.PushAsync(page);
			});
		BindingContext = this;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Force refresh the bindings to avoid stale command issues after back navigation
        BindingContext = null;
        BindingContext = this;
    }

}

