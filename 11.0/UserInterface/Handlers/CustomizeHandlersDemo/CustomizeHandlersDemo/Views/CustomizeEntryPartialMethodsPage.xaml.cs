namespace CustomizeHandlersDemo.Views;

public partial class CustomizeEntryPartialMethodsPage : ContentPage
{
    public CustomizeEntryPartialMethodsPage()
    {
        InitializeComponent();
    }

    partial void ChangedHandler(object sender, EventArgs e);
    partial void ChangingHandler(object sender, HandlerChangingEventArgs e);

    void OnEntryHandlerChanged(object sender, EventArgs e) => ChangedHandler(sender, e);
    void OnEntryHandlerChanging(object sender, HandlerChangingEventArgs e) => ChangingHandler(sender, e);
}