using CommunityToolkit.Maui;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.Maui.Platform;
using MonkeyCache;
using MonkeyCache.FileStore;
using Plugin.Maui.KeyListener;
using SkiaSharp.Views.Maui.Controls.Hosting;
using ZXing.Net.Maui;

#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif

#if ANDROID
[assembly: Android.App.UsesPermission(Android.Manifest.Permission.Camera)]
#endif

namespace PointOfSale;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseBarcodeReader()
			.UseMauiCommunityToolkit()
            .UseSkiaSharp()
            .UseKeyListener()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("opensans_semibold.ttf", "OpenSansSemiBold");
                fonts.AddFont("fa_solid.ttf", "FontAwesome");
                fonts.AddFont("fabmdl2.ttf", "Fabric");
            })
            .ConfigureMauiHandlers(handlers =>
            {
                ModifyEntry();
            });

        builder.Services.AddMauiBlazorWebView();
        
#if WINDOWS
            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddWindows(wndLifeCycleBuilder =>
                {
                    wndLifeCycleBuilder.OnWindowCreated(window =>
                    {
                        IntPtr nativeWindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                        WindowId win32WindowsId = Win32Interop.GetWindowIdFromWindow(nativeWindowHandle);
                        AppWindow winuiAppWindow = AppWindow.GetFromWindowId(win32WindowsId);

                        const int width = 1200;
                        const int height = 800;
                        int x = 1920 / 2 - width / 2; //Convert.ToInt32(DeviceDisplay.MainDisplayInfo.Width)
                        int y = 1080 / 2 - height / 2; //Convert.ToInt32(DeviceDisplay.MainDisplayInfo.Height)

                        winuiAppWindow.MoveAndResize(new RectInt32(x, y, width, height));
                    });
                });
            });
#endif

        Barrel.ApplicationId = "com.simplyprofound.pointofsale";

        return builder.Build();
	}

    public static void ModifyEntry()
    {
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoMoreBorders", (handler, view) =>
        {
#if ANDROID
            handler.PlatformView.SetBackgroundColor(Colors.Transparent.ToPlatform());
#elif IOS || MACCATALYST
            handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#elif WINDOWS
            // how can I remove the bottom border of the Entry?
            handler.PlatformView.FontWeight = Microsoft.UI.Text.FontWeights.Thin;
            handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
#endif
        });
    }
}
