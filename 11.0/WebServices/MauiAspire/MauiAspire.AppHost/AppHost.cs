var builder = DistributedApplication.CreateBuilder(args);

var webapi = builder.AddProject<Projects.SampleWebApi>("webapi");

var publicDevTunnel = builder.AddDevTunnel("devtunnel-public")
    .WithAnonymousAccess() // All ports on this tunnel default to allowing anonymous access
    .WithReference(webapi.GetEndpoint("https"));

var mauiapp = builder.AddMauiProject("mauiapp", "../SampleMauiApp/SampleMauiApp.csproj");
var blazorhybridapp = builder.AddMauiProject("blazorhybridapp", "../SampleBlazorHybridApp/SampleBlazorHybridApp.csproj");
var hybridwebviewapp = builder.AddMauiProject("hybridwebviewapp", "../SampleHybridWebViewApp/SampleHybridWebViewApp.csproj");

if (OperatingSystem.IsWindows())
{
    mauiapp.AddWindowsDevice()
        .WithReference(webapi);

    blazorhybridapp.AddWindowsDevice()
        .WithReference(webapi);

    hybridwebviewapp.AddWindowsDevice()
        .WithReference(webapi);
}

if (OperatingSystem.IsMacOS())
{
    mauiapp.AddMacCatalystDevice()
        .WithReference(webapi);

    mauiapp.AddiOSSimulator()
        .WithOtlpDevTunnel()
        .WithReference(webapi, publicDevTunnel);

    blazorhybridapp.AddMacCatalystDevice()
        .WithReference(webapi);

    blazorhybridapp.AddiOSSimulator()
        .WithOtlpDevTunnel()
        .WithReference(webapi, publicDevTunnel);

    hybridwebviewapp.AddMacCatalystDevice()
        .WithReference(webapi);

    hybridwebviewapp.AddiOSSimulator()
        .WithOtlpDevTunnel()
        .WithReference(webapi, publicDevTunnel);
}

mauiapp.AddAndroidEmulator()
    .WithOtlpDevTunnel()
    .WithReference(webapi, publicDevTunnel);

builder.Build().Run();
