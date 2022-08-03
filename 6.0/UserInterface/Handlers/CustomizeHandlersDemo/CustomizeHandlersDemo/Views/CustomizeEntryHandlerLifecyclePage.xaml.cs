#if ANDROID
using AndroidX.AppCompat.Widget;
#elif IOS || MACCATALYST
using UIKit;
#elif WINDOWS
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
#endif

namespace CustomizeHandlersDemo.Views;

public partial class CustomizeEntryHandlerLifecyclePage : ContentPage
{
	public CustomizeEntryHandlerLifecyclePage()
	{
		InitializeComponent();
	}

	void OnEntryHandlerChanged(object sender, EventArgs e)
	{
#if ANDROID
		((sender as Entry).Handler.PlatformView as AppCompatEditText).SetSelectAllOnFocus(true);
#elif IOS || MACCATALYST
		((sender as Entry).Handler.PlatformView as UITextField).EditingDidBegin += OnEditingDidBegin;
#elif WINDOWS
        ((sender as Entry).Handler.PlatformView as TextBox).GotFocus += OnGotFocus;
#endif
	}

	void OnEntryHandlerChanging(object sender, HandlerChangingEventArgs e)
	{
		if (e.OldHandler != null)
		{
#if IOS || MACCATALYST
			(e.OldHandler.PlatformView as UITextField).EditingDidBegin -= OnEditingDidBegin;
#elif WINDOWS
			(e.OldHandler.PlatformView as TextBox).GotFocus -= OnGotFocus;	
#endif
		}
	}

#if IOS || MACCATALYST                   
	void OnEditingDidBegin(object sender, EventArgs e)
	{
		var nativeView = sender as UITextField;
		nativeView.PerformSelector(new ObjCRuntime.Selector("selectAll"), null, 0.0f);
	}
#elif WINDOWS
	void OnGotFocus(object sender, RoutedEventArgs e)
	{
		var nativeView = sender as TextBox;
		nativeView.SelectAll();
	}
#endif
}