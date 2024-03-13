#if ANDROID
using PlatformApplication = Android.App.Application;
#elif IOS || MACCATALYST
using PlatformApplication = UIKit.IUIApplicationDelegate;
#elif WINDOWS
using PlatformApplication = Microsoft.UI.Xaml.Application;
#endif

namespace Microsoft.Maui.Controls;

internal class EmbeddedPlatformApplication : IPlatformApplication
{
    private readonly MauiContext rootContext;
    private readonly IMauiContext applicationContext;

    public IServiceProvider Services { get; }
    public IApplication Application { get; }

    public EmbeddedPlatformApplication(IServiceProvider services)
    {
        IPlatformApplication.Current = this;

#if ANDROID
        var platformApp = services.GetRequiredService<PlatformApplication>();
        rootContext = new MauiContext(services, platformApp);
#else
        rootContext = new MauiContext(services);
#endif

        applicationContext = MakeApplicationScope(rootContext);
        Services = applicationContext.Services;
        Application = Services.GetRequiredService<IApplication>();
    }

    private static IMauiContext MakeApplicationScope(IMauiContext rootContext)
    {
        var scopedContext = new MauiContext(rootContext.Services);
        InitializeScopedServices(scopedContext);
        return scopedContext;
    }

    private static void InitializeScopedServices(IMauiContext scopedContext)
    {
        var scopedServices = scopedContext.Services.GetServices<IMauiInitializeScopedService>();

        foreach (var service in scopedServices)
            service.Initialize(scopedContext.Services);
    }
}