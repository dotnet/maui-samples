namespace SafeAreaDemos.Views;

public partial class Example5Container : ContentPage
{
    public Example5Container()
    {
        InitializeComponent();
    }
    async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
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
