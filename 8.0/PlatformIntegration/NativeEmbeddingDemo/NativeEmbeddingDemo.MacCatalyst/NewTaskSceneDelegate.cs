using AppKit;

namespace NativeEmbeddingDemo.MacCatalyst
{
    [Register(nameof(NewTaskSceneDelegate))]
    public class NewTaskSceneDelegate : UIWindowSceneDelegate
    {
        public override UIWindow? Window { get; set; }

        [Export("scene:willConnectToSession:options:")]
        public override void WillConnect(UIScene scene, UISceneSession session, UISceneConnectionOptions connectionOptions)
        {
            if (scene is not UIWindowScene windowScene)
                return;

            var window = new UIWindow(windowScene);
            window.RootViewController = new NewTaskViewController();
            window.WindowScene!.Title = "New Task";
            window.MakeKeyAndVisible();

            Window = window;

            if (OperatingSystem.IsMacCatalyst())
            {
                ConfigureMacWindowSize();
                ConfigureToolbar();
            }
        }

        void ConfigureMacWindowSize()
        {
            if (Window?.WindowScene?.SizeRestrictions is null)
                return;

            var fixedSize = new CGSize(400, 250);
            Window.WindowScene.SizeRestrictions.MinimumSize = fixedSize;
            Window.WindowScene.SizeRestrictions.MaximumSize = fixedSize;
        }

        void ConfigureToolbar()
        {
            if (Window?.WindowScene?.Titlebar is null)
                return;

            var toolbar = new NSToolbar();
            toolbar.ShowsBaselineSeparator = false;

            var titlebar = Window.WindowScene.Titlebar;
            titlebar.Toolbar = toolbar;
            titlebar.ToolbarStyle = UITitlebarToolbarStyle.Automatic;
            titlebar.TitleVisibility = UITitlebarTitleVisibility.Visible;
        }
    }
}

