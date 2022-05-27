using MauiEmbedding.Shared;
using Microsoft.Maui.Platform;
using AppHostBuilderExtensions = Microsoft.Maui.Embedding.AppHostBuilderExtensions;

namespace MauiEmbedding.MacCatalyst;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {
	public override UIWindow? Window {
		get;
		set;
	}

	MauiContext mauiContext;

	public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
	{
		// create a new window instance based on the screen size
		Window = new UIWindow (UIScreen.MainScreen.Bounds);

		// create a UIViewController with a single UILabel
		var vc = new UIViewController ();
		vc.View!.AddSubview (new UILabel (Window!.Frame) {
			BackgroundColor = UIColor.SystemBackground,
			TextAlignment = UITextAlignment.Center,
			Text = "Hello, Catalyst!",
			AutoresizingMask = UIViewAutoresizing.All,
		});
		Window.RootViewController = vc;

		// Setup .NET MAUI
            var builder = MauiApp.CreateBuilder();

            // Add Microsoft.Maui Controls
            AppHostBuilderExtensions.UseMauiEmbedding<Microsoft.Maui.Controls.Application>(builder);

            var mauiApp = builder.Build();

            // Create and save a Maui Context. This is needed for creating Platform UI
            mauiContext = new MauiContext(mauiApp.Services);
            var mauiControl = new MauiControl().ToPlatform(mauiContext);
            mauiControl.Center = Window.Center;
            vc.View!.AddSubview(mauiControl);

		// make the window visible
		Window.MakeKeyAndVisible ();

		return true;
	}
}

