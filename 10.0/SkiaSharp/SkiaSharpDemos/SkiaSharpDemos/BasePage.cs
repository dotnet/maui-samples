using System.Windows.Input;

namespace SkiaSharpDemos
{
	public class BasePage : ContentPage
	{
        public ICommand NavigateCommand { get; private set; }

        public BasePage()
        {
            NavigateCommand = new Command<Type>(async (Type pageType) =>
            {
                Page page = (Page)Activator.CreateInstance(pageType);
                await Navigation.PushAsync(page);
            });

            BindingContext = this;
        }
    }
}
