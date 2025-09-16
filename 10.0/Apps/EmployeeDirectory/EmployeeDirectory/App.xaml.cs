using EmployeeDirectory.Views.CSharp;
using EmployeeDirectory.Views.Xaml;
using EmployeeDirectory.Core.Services;
using System.Diagnostics;

namespace EmployeeDirectory;

public enum UIImplementation 
{
    CSharp = 0,
    Xaml
}

public partial class App : Application
{
    // Change the following line to switch between XAML and C# versions
    private static UIImplementation uiImplementation = UIImplementation.Xaml;

    public static IDirectoryService? Service { get; set; }

    public static IPhoneFeatureService? PhoneFeatureService { get; set; }

    public static DateTime LastUseTime { get; set; }

    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Return a window with a lightweight loading page; do not block UI thread.
        var loadingPage = new ContentPage
        {
            Title = "Loading",
            Content = new VerticalStackLayout
            {
                Spacing = 12,
                Padding = 24,
                Children =
                {
                    new ActivityIndicator { IsRunning = true, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center },
                    new Label { Text = "Loading directory...", HorizontalOptions = LayoutOptions.Center }
                }
            }
        };

        var nav = new NavigationPage(loadingPage);

        // Dispatch after initial layout to avoid fragment replacement issues on Android
        nav.Dispatcher.Dispatch(async () => await InitializeDirectoryAsync(nav));

        return new Window(nav);
    }

    private async Task InitializeDirectoryAsync(NavigationPage nav)
    {
        try
        {
            if (Service == null)
            {
                Debug.WriteLine("[App] Starting directory CSV load...");
                Service = await MemoryDirectoryService.FromCsv("XamarinDirectory.csv").ConfigureAwait(false);
                Debug.WriteLine("[App] Directory loaded.");
            }

            var employeeList = uiImplementation == UIImplementation.CSharp
                ? (ContentPage)new EmployeeListView()
                : new EmployeeListXaml();

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var loadingRoot = nav.RootPage; // current first page (loading)
                if (loadingRoot == null)
                {
                    // Fallback: just push
                    await nav.PushAsync(employeeList, false);
                    return;
                }

                // Safe replacement pattern: insert new page before loading then pop loading
                nav.Navigation.InsertPageBefore(employeeList, loadingRoot);
                await nav.PopAsync(false); // removes loadingRoot
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine("[App] Initialization failed: " + ex);
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var errorPage = new ContentPage
                {
                    Title = "Error",
                    Content = new ScrollView
                    {
                        Content = new VerticalStackLayout
                        {
                            Padding = 24,
                            Children =
                            {
                                new Label { Text = "Failed to load employee directory.", FontAttributes = FontAttributes.Bold, TextColor = Colors.Red },
                                new Label { Text = ex.Message, FontSize = 12 },
                                new Button
                                {
                                    Text = "Retry",
                                    Command = new Command(() => nav.Dispatcher.Dispatch(async () => await InitializeDirectoryAsync(nav)))
                                }
                            }
                        }
                    }
                };

                var loadingRoot = nav.RootPage;
                if (loadingRoot != null)
                {
                    nav.Navigation.InsertPageBefore(errorPage, loadingRoot);
                    await nav.PopAsync(false);
                }
                else
                {
                    await nav.PushAsync(errorPage, false);
                }
            });
        }
    }
}