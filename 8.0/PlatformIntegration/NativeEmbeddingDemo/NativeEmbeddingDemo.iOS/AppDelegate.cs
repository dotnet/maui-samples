namespace NativeEmbeddingDemo.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow? Window { get; set; }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            Window = new UIWindow(UIScreen.MainScreen.Bounds);
            var vc = new MainViewController();
            Window.RootViewController = vc;
            Window.MakeKeyAndVisible();
            return true;
        }
    }
}
