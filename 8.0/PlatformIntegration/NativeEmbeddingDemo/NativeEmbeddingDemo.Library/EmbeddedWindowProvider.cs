#if ANDROID
using PlatformWindow = Android.App.Activity;
#elif IOS || MACCATALYST
using PlatformWindow = UIKit.UIWindow;
#elif WINDOWS
using PlatformWindow = Microsoft.UI.Xaml.Window;
#endif

namespace Microsoft.Maui.Controls;

public class EmbeddedWindowProvider
{
    WeakReference<PlatformWindow?>? platformWindow;
    WeakReference<Window?>? window;

    public PlatformWindow? PlatformWindow => Get(platformWindow);
    public Window? Window => Get(window);

    public void SetWindow(PlatformWindow? platformWindow, Window? window)
    {
        this.platformWindow = new WeakReference<PlatformWindow?>(platformWindow);
        this.window = new WeakReference<Window?>(window);
    }

    private static T? Get<T>(WeakReference<T?>? weak) where T : class =>
        weak is not null && weak.TryGetTarget(out var target) ? target : null;
}