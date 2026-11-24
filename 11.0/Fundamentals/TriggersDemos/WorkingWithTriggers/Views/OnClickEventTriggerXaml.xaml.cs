namespace WorkingWithTriggers
{
	public partial class OnClickEventTriggerXaml : ContentPage
	{
		public OnClickEventTriggerXaml ()
		{
			InitializeComponent ();
		}

		async void OnDoThisClick (object sender, EventArgs ea) 
		{
			await DisplayAlertAsync ("Do this", "alert displays as well as TriggerAction", "Cool");
		}

		async void OnDoThatClick (object sender, EventArgs ea) 
		{
			await DisplayAlertAsync ("Do that", "alert displays as well as TriggerAction", "Cool");
		}
	}
}

