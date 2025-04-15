using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Navigation;
using AndroidX.Navigation.UI;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace NativeEmbeddingDemo.Droid
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Theme = "@style/Theme.MyApplication")]
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
                var snackBar = Snackbar.Make(fab, "Replace with your own action", Snackbar.LengthLong);
                snackBar.SetAnchorView(Resource.Id.fab);
                snackBar.SetAction("Action", _ => { });
                snackBar.Show();
            };
        }

        public override bool OnCreateOptionsMenu(IMenu? menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var id = item.ItemId;
            if (id == Resource.Id.SettingsFragment)
            {
                var navController = Navigation.FindNavController(this, Resource.Id.nav_host_fragment_content_main);
                if (NavigationUI.OnNavDestinationSelected(item, navController))
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public override bool OnSupportNavigateUp()
        {
            var navController = Navigation.FindNavController(this, Resource.Id.nav_host_fragment_content_main);
            return NavigationUI.NavigateUp(navController, appBarConfiguration!) || base.OnSupportNavigateUp();
        }
    }
}