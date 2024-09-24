using Android.Runtime;
using Android.Views;
using AndroidX.Navigation.Fragment;
using Microsoft.Maui.Controls.Embedding;
using static Android.Views.ViewGroup.LayoutParams;
using Button = Android.Widget.Button;
using Fragment = AndroidX.Fragment.App.Fragment;
using View = Android.Views.View;

namespace NativeEmbeddingDemo.Droid;

[Register("com.companyname.nativeembeddingdemo." + nameof(FirstFragment))]
public class FirstFragment : Fragment
{
    Activity? _window;
    IMauiContext? _windowContext;
    MyMauiContent? _mauiView;
    Android.Views.View? _nativeView;

    public IMauiContext WindowContext =>
        _windowContext ??= MyEmbeddedMauiApp.Shared.CreateEmbeddedWindowContext(_window ?? throw new InvalidOperationException());
 
    public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState) =>
        inflater.Inflate(Resource.Layout.fragment_first, container, false);

    public override void OnViewCreated(View view, Bundle? savedInstanceState)
    {
        base.OnViewCreated(view, savedInstanceState);
        _window ??= Activity;

        // Create Android button
        var androidButton = view.FindViewById<Button>(Resource.Id.button_first)!;
        androidButton.Click += (s, e) =>
        {
            NavHostFragment.FindNavController(this).Navigate(Resource.Id.action_FirstFragment_to_SecondFragment);
        };

        var animateButton = view.FindViewById<Button>(Resource.Id.button_animate)!;
        animateButton.Click += OnAndroidButtonClicked;

        //// App context
        //// Ensure .NET MAUI app is built before creating .NET MAUI views
        //var mauiApp = MauiProgram.CreateMauiApp();

        //// Create .NET MAUI context
        //var mauiContext = new MauiContext(mauiApp.Services, Activity);

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
        var rootLayout = view.FindViewById<LinearLayout>(Resource.Id.layout_first)!;
        rootLayout.AddView(_nativeView, 1, new LinearLayout.LayoutParams(MatchParent, WrapContent));
    }

    public override void OnDestroyView()
    {
        base.OnDestroyView();

        // Remove the view from the UI
        var rootLayout = View!.FindViewById<LinearLayout>(Resource.Id.layout_first)!;
        rootLayout.RemoveView(_nativeView);

        // Cleanup any Window
        if (_mauiView?.Window is IWindow window)
            window.Destroying();

        base.OnStop();
    }

    async void OnAndroidButtonClicked(object? sender, EventArgs e)
    {
        if (_mauiView?.DotNetBot is not Image bot)
            return;

        await bot.RotateTo(360, 1000);
        bot.Rotation = 0;

        bot.HeightRequest = 90;
    }

}