using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CustomizeHandlersDemo.Views
{
    public partial class CustomizeEntryPartialMethodsPage : ContentPage
    {
        partial void ChangedHandler(object sender, EventArgs e)
        {
            Entry entry = sender as Entry;
            (entry.Handler.PlatformView as TextBox).GotFocus += OnGotFocus;
        }

        partial void ChangingHandler(object sender, HandlerChangingEventArgs e)
        {
            if (e.OldHandler != null)
            {
                (e.OldHandler.PlatformView as TextBox).GotFocus -= OnGotFocus;
            }
        }

        void OnGotFocus(object sender, RoutedEventArgs e)
        {
            var nativeView = sender as TextBox;
            nativeView.SelectAll();
        }
    }
}
