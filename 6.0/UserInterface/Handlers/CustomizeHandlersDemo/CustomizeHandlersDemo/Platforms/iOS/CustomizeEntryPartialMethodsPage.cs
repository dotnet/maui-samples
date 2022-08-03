using UIKit;

namespace CustomizeHandlersDemo.Views
{
    public partial class CustomizeEntryPartialMethodsPage : ContentPage
    {
        partial void ChangedHandler(object sender, EventArgs e)
        {
            ((sender as Entry).Handler.PlatformView as UITextField).EditingDidBegin += OnEditingDidBegin;
        }

        partial void ChangingHandler(object sender, HandlerChangingEventArgs e)
        {
            if (e.OldHandler != null)
            {
                (e.OldHandler.PlatformView as UITextField).EditingDidBegin -= OnEditingDidBegin;
            }
        }

        void OnEditingDidBegin(object sender, EventArgs e)
        {
            var nativeView = sender as UITextField;
            nativeView.PerformSelector(new ObjCRuntime.Selector("selectAll"), null, 0.0f);
        }
    }
}
