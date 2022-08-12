using AndroidX.AppCompat.Widget;

namespace CustomizeHandlersDemo.Views
{
    public partial class CustomizeEntryPartialMethodsPage : ContentPage
    {
        partial void ChangedHandler(object sender, EventArgs e)
        {
            ((sender as Entry).Handler.PlatformView as AppCompatEditText).SetSelectAllOnFocus(true);
        }
    }
}
