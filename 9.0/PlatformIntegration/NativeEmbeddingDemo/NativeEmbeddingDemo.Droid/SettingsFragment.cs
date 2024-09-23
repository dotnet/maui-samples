﻿using Android.Runtime;
using Android.Views;
using AndroidX.Navigation.Fragment;
using static Android.Views.ViewGroup.LayoutParams;
using Button = Android.Widget.Button;
using Fragment = AndroidX.Fragment.App.Fragment;
using View = Android.Views.View;

namespace NativeEmbeddingDemo.Droid;

[Register("com.companyname.nativeembeddingdemo." + nameof(SettingsFragment))]
public class SettingsFragment : Fragment
{
    MyMauiContent? _mauiView;
    Android.Views.View? _nativeView;

    public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState) =>
        inflater.Inflate(Resource.Layout.fragment_settings, container, false);

    public override void OnViewCreated(View view, Bundle? savedInstanceState)
    {
        base.OnViewCreated(view, savedInstanceState);

        var settingsButton = view.FindViewById<Button>(Resource.Id.button_settings)!;
        settingsButton.Click += (s, e) =>
        {
            NavHostFragment.FindNavController(this).NavigateUp();
        };

        var animateButton = view.FindViewById<Button>(Resource.Id.button_animate);
        animateButton.Click += OnAndroidButtonClicked;

        // App context
        // Ensure .NET MAUI app is built before creating .NET MAUI views
        var mauiApp = MauiProgram.CreateMauiApp();

        // Create .NET MAUI context
        var mauiContext = new MauiContext(mauiApp.Services, this);

        // Create .NET MAUI content
        _mauiView = new MyMauiContent();

        // Create native view
        _nativeView = _mauiView.ToPlatformEmbedded(mauiContext);

        // Add native view to layout
        var rootLayout = view.FindViewById<LinearLayout>(Resource.Id.layout_settings)!;
        rootLayout.AddView(_nativeView, 1, new LinearLayout.LayoutParams(MatchParent, WrapContent));
    }

    public override void OnDestroyView()
	{
		base.OnDestroyView();

		// Remove the view from the UI
		var rootLayout = View!.FindViewById<LinearLayout>(Resource.Id.layout_settings)!;
		rootLayout.RemoveView(_nativeView);

		// If we used a window, then clean that up
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