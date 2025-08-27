using System;


namespace TabbedPageWithNavigationPage
{
	public partial class UpcomingAppointmentsPage : ContentPage
	{
		public UpcomingAppointmentsPage ()
		{
			InitializeComponent ();
		}

		async void OnBackButtonClicked (object sender, EventArgs e)
		{
			await Navigation.PopAsync ();
		}
	}
}

