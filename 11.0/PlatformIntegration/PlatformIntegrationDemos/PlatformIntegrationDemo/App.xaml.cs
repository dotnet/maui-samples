using PlatformIntegrationDemo.Views;
using System.Runtime.CompilerServices;

namespace PlatformIntegrationDemo;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        return new Window(new AppShell());
    }

    public static void HandleAppActions(AppAction appAction)
    {
        App.Current.Dispatcher.Dispatch(async () =>
        {
            var page = appAction.Id switch
            {
                "battery_info" => new BatteryPage(),
                "app_info" => new AppInfoPage(),
                _ => default(Page)
            };

            if (page != null)
            {
                await Application.Current.Windows[0].Page.Navigation.PopToRootAsync();
                await Application.Current.Windows[0].Page.Navigation.PushAsync(page);
            }
        });
    }
}
