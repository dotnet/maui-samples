using CustomizeHandlersDemo.Controls;

namespace CustomizeHandlersDemo.Views;

public partial class CustomizeSpecificEntryPage : ContentPage
{
	public CustomizeSpecificEntryPage()
	{
		InitializeComponent();
        ModifyEntry();
	}

    void ModifyEntry()
    {
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("MyCustomization", (handler, view) =>
        {
            if (view is MyEntry)
            {
#if ANDROID
                handler.PlatformView.SetSelectAllOnFocus(true);
#elif IOS || MACCATALYST
                handler.PlatformView.EditingDidBegin += (s, e) =>
                {
                    handler.PlatformView.PerformSelector(new ObjCRuntime.Selector("selectAll"), null, 0.0f);
                };
#elif WINDOWS
                handler.PlatformView.GotFocus += (s, e) =>
                {
                    handler.PlatformView.SelectAll();
                };
#endif
            }
        });
    }

}