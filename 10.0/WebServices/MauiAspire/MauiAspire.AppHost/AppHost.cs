var builder = DistributedApplication.CreateBuilder(args);

var webapi = builder.AddProject<Projects.SampleWebApi>("webapi");

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

    blazorhybridapp.AddMacCatalystDevice()
        .WithReference(webapi);

    hybridwebviewapp.AddMacCatalystDevice()
        .WithReference(webapi);
}

builder.Build().Run();
