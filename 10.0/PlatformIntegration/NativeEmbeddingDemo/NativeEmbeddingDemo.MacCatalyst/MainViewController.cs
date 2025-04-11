using Microsoft.Maui.Controls.Embedding;

namespace NativeEmbeddingDemo.MacCatalyst;

public class MainViewController : UIViewController
{
    public static class MyEmbeddedMauiApp
    {
        static MauiApp? _shared;

        public static MauiApp Shared =>
            _shared ??= MauiProgram.CreateMauiApp();
    }

    UIKit.UIWindow? _window;
    IMauiContext? _windowContext;
    MyMauiContent? _mauiView;
    UIView? _nativeView;

    public IMauiContext WindowContext =>
        _windowContext ??= MyEmbeddedMauiApp.Shared.CreateEmbeddedWindowContext(_window ?? throw new InvalidOperationException());

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        _window ??= ParentViewController!.View!.Window;

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

        //// App context
        //// Ensure .NET MAUI app is built before creating .NET MAUI views
        //var mauiApp = MauiProgram.CreateMauiApp();

        //// Create .NET MAUI context
        //var mauiContext = new MauiContext(mauiApp.Services);

        //// Create .NET MAUI content
        //_mauiView = new MyMauiContent();

        //// Create native view
        //_nativeView = _mauiView.ToPlatformEmbedded(mauiContext);

        // Window context
        // Create MAUI embedded window context
        var context = WindowContext;

        // Create .NET MAUI content
        _mauiView = new MyMauiContent();

        // Create native view
        _nativeView = _mauiView.ToPlatformEmbedded(context);

        // Add native view to layout
        stackView.AddArrangedSubview(new ContainerView(_nativeView));

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
        if (_mauiView?.DotNetBot is not Image bot)
            return;

        await bot.RotateTo(360, 1000);
        bot.Rotation = 0;

        bot.HeightRequest = 90;
    }

    // UIStackView uses IntrinsicContentSize instead of SizeThatFits so
    // create a container view to wrap the .NET MAUI view and redirect
    // the IntrinsicContentSize to the .NET MAUI view's SizeThatFits.
    class ContainerView : UIView
    {
        public ContainerView(UIView view)
        {
            AddSubview(view);
        }

        public override CGSize IntrinsicContentSize =>
            SizeThatFits(new CGSize(nfloat.MaxValue, nfloat.MaxValue));

        public override CGSize SizeThatFits(CGSize size) =>
            Subviews?.FirstOrDefault()?.SizeThatFits(size) ?? CGSize.Empty;

        public override void LayoutSubviews()
        {
            if (Subviews?.FirstOrDefault() is { } view)
                view.Frame = Bounds;
        }

        public override void SetNeedsLayout()
        {
            base.SetNeedsLayout();
            InvalidateIntrinsicContentSize();
        }
    }
}

