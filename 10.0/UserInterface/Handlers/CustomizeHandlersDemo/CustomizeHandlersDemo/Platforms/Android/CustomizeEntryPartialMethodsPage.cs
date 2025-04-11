using AndroidX.AppCompat.Widget;

namespace CustomizeHandlersDemo.Views
{
    public partial class CustomizeEntryPartialMethodsPage : ContentPage
    {
        partial void ChangedHandler(object sender, EventArgs e)
        {
            Entry entry = sender as Entry;
            (entry.Handler.PlatformView as AppCompatEditText).SetSelectAllOnFocus(true);
        }
    }
}
