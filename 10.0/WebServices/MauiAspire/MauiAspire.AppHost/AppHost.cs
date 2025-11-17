var builder = DistributedApplication.CreateBuilder(args);

var webapi = builder.AddProject<Projects.SampleWebApi>("webapi");

var mauiapp = builder.AddMauiProject("mauiapp", "../SampleMauiApp/SampleMauiApp.csproj");

mauiapp.AddWindowsDevice()
    .WithReference(webapi);

mauiapp.AddMacCatalystDevice()
    .WithReference(webapi);

var blazorhybridapp = builder.AddMauiProject("blazorhybridapp", "../SampleBlazorHybridApp/SampleBlazorHybridApp.csproj");

blazorhybridapp.AddWindowsDevice()
    .WithReference(webapi);

blazorhybridapp.AddMacCatalystDevice()
    .WithReference(webapi);

var hybridwebviewapp = builder.AddMauiProject("hybridwebviewapp", "../SampleHybridWebViewApp/SampleHybridWebViewApp.csproj");

hybridwebviewapp.AddWindowsDevice()
    .WithReference(webapi);

hybridwebviewapp.AddMacCatalystDevice()
    .WithReference(webapi);

builder.Build().Run();
