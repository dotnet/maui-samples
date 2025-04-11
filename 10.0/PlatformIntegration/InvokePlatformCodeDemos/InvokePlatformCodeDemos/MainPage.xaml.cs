using InvokePlatformCodeDemos.Services;
using ConditionalCompilationDeviceOrientationService = InvokePlatformCodeDemos.Services.ConditionalCompilation.DeviceOrientationService;
using PartialMethodsDeviceOrientationService = InvokePlatformCodeDemos.Services.PartialMethods.DeviceOrientationService;

namespace InvokePlatformCodeDemos;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var deviceOrientationService1 = new ConditionalCompilationDeviceOrientationService();
        DeviceOrientation orientation1 = deviceOrientationService1.GetOrientation();
        conditionalCompilationOrientationLabel.Text = orientation1.ToString();

        var deviceOrientationService2 = new PartialMethodsDeviceOrientationService();
        DeviceOrientation orientation2 = deviceOrientationService2.GetOrientation();
        partialMethodsOrientationLabel.Text = orientation2.ToString();    
    }
}
