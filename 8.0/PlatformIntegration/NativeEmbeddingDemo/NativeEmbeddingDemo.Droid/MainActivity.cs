using static Android.Views.ViewGroup.LayoutParams;

namespace NativeEmbeddingDemo.Droid
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Theme = "@style/AppTheme")]
    public class MainActivity : Activity
    {
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

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set the view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            var rootLayout = FindViewById<LinearLayout>(Resource.Id.rootLayout)!;

            // Create Android button
            var androidButton = new Android.Widget.Button(this);
            androidButton.Text = "Android button above .NET MAUI controls";
            androidButton.Click += OnAndroidButtonClicked;
            rootLayout.AddView(androidButton, new LinearLayout.LayoutParams(MatchParent, WrapContent));

            // Ensure .NET MAUI app is built before creating .NET MAUI views
            var mauiApp = MainActivity.MauiApp.Value;

            // Create .NET MAUI context
            var mauiContext = UseWindowContext
                ? mauiApp.CreateEmbeddedWindowContext(this) // Create window context
                : new MauiContext(mauiApp.Services, this);  // Create app context

            // Create .NET MAUI content
            mauiView = new MyMauiContent();

            // Create native view
            var nativeView = mauiView.ToPlatformEmbedded(mauiContext);

            // Add native view to layout
            rootLayout.AddView(nativeView, new LinearLayout.LayoutParams(MatchParent, WrapContent));
        }

        async void OnAndroidButtonClicked(object? sender, EventArgs e)
        {
            if (mauiView?.DotNetBot is not Image bot)
                return;

            await bot.RotateTo(360, 1000);
            bot.Rotation = 0;

            bot.HeightRequest = 90;
        }
    }
}