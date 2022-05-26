using MauiEmbedding.Shared;
using Microsoft.Maui.Platform;
using AppHostBuilderExtensions = Microsoft.Maui.Embedding.AppHostBuilderExtensions;

namespace MauiEmbedding.Android
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        MauiContext mauiContext;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Setup .NET MAUI
            var builder = MauiApp.CreateBuilder();

            // Add Microsoft.Maui Controls
            AppHostBuilderExtensions.UseMauiEmbedding<Microsoft.Maui.Controls.Application>(builder);

            var mauiApp = builder.Build();

            // Create and save a Maui Context. This is needed for creating Platform UI
            mauiContext = new MauiContext(mauiApp.Services, this);

            //Create a Maui Page
            var myMauiPage = new MauiPage();

            // Example A: Turn the Microsoft.Maui page into an Android view
            //var view = myMauiPage.ToPlatform(mauiContext);
            //SetContentView(view);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // Example B: Turn any Microsoft.Maui control into an Android view
            FindViewById<LinearLayout>(Resource.Id.xplatTarget)
                .AddView(
                    new MauiControl()
                        .ToPlatform(mauiContext)
                );
        }
    }
}