using Microsoft.Maui.Platform;

namespace NativeEmbeddingDemo.MacCatalyst
{
    public class MainViewController : UIViewController
    {
        UIWindow GetWindow() =>
            View?.Window ??
            ParentViewController?.View?.Window ??
            MainViewController.MauiApp.Value.Services.GetRequiredService<IUIApplicationDelegate>().GetWindow() ??
            UIApplication.SharedApplication.Delegate.GetWindow();

        public static readonly Lazy<MauiApp> MauiApp = new(() =>
        {
            var mauiApp = MauiProgram.CreateMauiApp(builder =>
            {
                builder.UseMauiEmbedding();
            });
            return mauiApp;
        });

        public static bool UseWindowContext = true;

        MyMauiContent? mauiView;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Main view controller";

            View!.BackgroundColor = UIColor.SystemBackground;

            var stackView = new UIStackView
            {
                Axis = UILayoutConstraintAxis.Vertical,
                Alignment = UIStackViewAlignment.Fill,
                Distribution = UIStackViewDistribution.Fill,
                Spacing = 8,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            View.AddSubview(stackView);

            NSLayoutConstraint.ActivateConstraints(new[]
            {
                stackView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 20),
                stackView.LeadingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeadingAnchor, 20),
                stackView.TrailingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TrailingAnchor, -20),
                stackView.BottomAnchor.ConstraintLessThanOrEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -20)
            });

            // Create UIKit button
            var uiButton = new UIButton(UIButtonType.System);
            uiButton.SetTitle("UIKit button above .NET MAUI controls", UIControlState.Normal);
            uiButton.TouchUpInside += OnUIButtonClicked;
            stackView.AddArrangedSubview(uiButton);

            // Ensure .NET MAUI app is built before creating .NET MAUI views
            var mauiApp = MainViewController.MauiApp.Value;

            // Create .NET MAUI context
            var mauiContext = UseWindowContext
                ? mauiApp.CreateEmbeddedWindowContext(GetWindow()) // Create window context
                : new MauiContext(mauiApp.Services);               // Create app context


            // Create .NET MAUI content
            mauiView = new MyMauiContent();

            // Create native view
            var nativeView = mauiView.ToPlatformEmbedded(mauiContext);
            nativeView.WidthAnchor.ConstraintEqualTo(View.Frame.Width).Active = true;
            nativeView.HeightAnchor.ConstraintEqualTo(500).Active = true;

            // Add native view to layout
            stackView.AddArrangedSubview(nativeView);

            AddNavBarButtons();
        }

        void AddNavBarButtons()
        {
            var addNewWindowButton = new UIBarButtonItem(
                UIImage.GetSystemImage("macwindow.badge.plus"),
                UIBarButtonItemStyle.Plain,
                (sender, e) => RequestSession());

            var addNewTaskButton = new UIBarButtonItem(
                UIBarButtonSystemItem.Add,
                (sender, e) => RequestSession("NewTaskWindow"));

            NavigationItem.RightBarButtonItems = [addNewTaskButton, addNewWindowButton];
        }

        void RequestSession(string? activityType = null)
        {
            var activity = activityType is null
                ? null
                : new NSUserActivity(activityType);

            if (OperatingSystem.IsMacCatalystVersionAtLeast(17))
            {
                var request = UISceneSessionActivationRequest.Create();
                request.UserActivity = activity;

                UIApplication.SharedApplication.ActivateSceneSession(request, error =>
                {
                    Console.WriteLine(new NSErrorException(error));
                });
            }
            else
            {
                UIApplication.SharedApplication.RequestSceneSessionActivation(null, activity, null, error =>
                {
                    Console.WriteLine(new NSErrorException(error));
                });
            }
        }

        async void OnUIButtonClicked(object? sender, EventArgs e)
        {
            if (mauiView?.DotNetBot is not Image bot)
                return;

            await bot.RotateTo(360, 1000);
            bot.Rotation = 0;

            bot.HeightRequest = 90;
        }
    }
}

