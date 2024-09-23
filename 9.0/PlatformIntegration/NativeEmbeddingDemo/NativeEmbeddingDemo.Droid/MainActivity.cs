using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Navigation;
using AndroidX.Navigation.UI;
using Google.Android.Material.AppBar;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;

namespace NativeEmbeddingDemo.Droid
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {
        AppBarConfiguration? appBarConfiguration;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set the view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            var navController = Navigation.FindNavController(this, Resource.Id.nav_host_fragment_content_main);
            appBarConfiguration = new AppBarConfiguration.Builder(navController.Graph).Build();
            NavigationUI.SetupActionBarWithNavController(this, navController, appBarConfiguration);

            var fab = FindViewById<FloatingActionButton>(Resource.Id.fab)!;
            fab.Click += (s, e) =>
            {
                var snackBar = snackBar.Make(fab, "Replace with your own action", SnackBar.LengthLong);
                snackBar.SetAnchorView(Resource.Id.fab);
                snackBar.SetAction("Action", _ => { });
                snackBar.Show();
            };
        }
    }
}