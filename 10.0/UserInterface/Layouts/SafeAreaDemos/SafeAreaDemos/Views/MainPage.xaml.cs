using System.Windows.Input;

namespace SafeAreaDemos
{
    public partial class MainPage : ContentPage
	{
        public ICommand NavigateCommand { get; private set; }
        public ICommand NavigateToNavPageCommand { get; private set; }

        public MainPage()
		{
			InitializeComponent();

            NavigateCommand = new Command<Type>(
                async (Type pageType) =>
                {
                    Page page = (Page)Activator.CreateInstance(pageType);
                    await Navigation.PushAsync(page);
                });

            NavigateToNavPageCommand = new Command<Type>(
                async (Type pageType) =>
                {
                    Page page = (Page)Activator.CreateInstance(pageType);
                    // Wrap in a NavigationPage with transparent bar for these examples
                    var navPage = new NavigationPage(page)
                    {
                        BarBackgroundColor = Color.FromRgba(255, 255, 255, 128), // Semi-transparent white
                        BarTextColor = Colors.Black
                    };
                    await Navigation.PushModalAsync(navPage);
                });

            BindingContext = this;
        }
	}
}
