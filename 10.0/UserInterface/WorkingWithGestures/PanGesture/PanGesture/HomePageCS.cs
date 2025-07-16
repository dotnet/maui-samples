

namespace PanGesture
{
	public class HomePageCS : ContentPage
	{
		public HomePageCS ()
		{
			Content = new AbsoluteLayout {
				Padding = new Thickness (20),
				Children = {
					new PanContainer {
						Content = new Image {
							Source = ImageSource.FromFile ("mono_monkey.jpg"),
							WidthRequest = 1024,
							HeightRequest = 768
						}
					}	
				}
			};
		}
	}
}
