using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;

namespace HelloMaui
{
	public class MainWindow : IWindow
	{
		public MainWindow()
		{
			Page = new MainPage();
		}

		public IPage Page { get; set; }

		public IMauiContext MauiContext { get; set; }
	}
}
