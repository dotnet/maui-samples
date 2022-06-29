using System.Windows.Input;

namespace FlexLayoutDemos
{
	public partial class MainPage : ContentPage
	{
        public ICommand NavigateCommand { private set; get; }

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
    }
}
