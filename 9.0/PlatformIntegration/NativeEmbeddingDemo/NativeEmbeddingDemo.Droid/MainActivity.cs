using static Android.Views.ViewGroup.LayoutParams;

namespace NativeEmbeddingDemo.Droid
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Theme = "@style/AppTheme")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set the view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            var rootLayout = FindViewById<LinearLayout>(Resource.Id.rootLayout)!;


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

    }
}