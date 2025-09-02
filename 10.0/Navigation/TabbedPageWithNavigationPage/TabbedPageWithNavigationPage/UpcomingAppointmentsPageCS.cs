using System;


namespace TabbedPageWithNavigationPage
{
	public class UpcomingAppointmentsPageCS : ContentPage
	{
		public UpcomingAppointmentsPageCS ()
		{
			var button = new Button {
				Text = "Back",
				VerticalOptions = LayoutOptions.Center
			};
			button.Clicked += OnBackButtonClicked;

			Title = "Upcoming";
			Content = new StackLayout { 
				Children = {
					new Label {
						Text = "Upcoming appointments go here",
						HorizontalOptions = LayoutOptions.Center,
						VerticalOptions = LayoutOptions.Center
					},
					button
				}
			};
		}

		async void OnBackButtonClicked (object sender, EventArgs e)
		{
			await Navigation.PopAsync ();
		}
	}
}
