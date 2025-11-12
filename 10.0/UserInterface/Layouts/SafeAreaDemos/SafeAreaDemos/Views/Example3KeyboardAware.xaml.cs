namespace SafeAreaDemos.Views;

public partial class Example3KeyboardAware : ContentPage
{
	public Example3KeyboardAware()
	{
		InitializeComponent();
	}
	
	protected override void OnAppearing()
	{
		base.OnAppearing();
		
		// Auto-focus the message entry to show keyboard for screenshot
		Dispatcher.DispatchDelayed(TimeSpan.FromSeconds(2), () =>
		{
			var entry = this.FindByName<Entry>("MessageEntry");
			entry?.Focus();
		});
	}
}
