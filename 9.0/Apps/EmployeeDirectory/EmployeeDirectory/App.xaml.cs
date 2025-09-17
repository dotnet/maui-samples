using EmployeeDirectory.Views.CSharp;
using EmployeeDirectory.Views.Xaml;
using EmployeeDirectory.Core.Services;

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
        // Initialize service synchronously by using GetAwaiter().GetResult()
        // This is a workaround for the synchronous CreateWindow method
        Service = MemoryDirectoryService.FromCsv("XamarinDirectory.csv").GetAwaiter().GetResult();

        ContentPage employeeList;

        if (uiImplementation == UIImplementation.CSharp)
            employeeList = new EmployeeListView();
        else
            employeeList = new EmployeeListXaml();

        return new Window(new NavigationPage(employeeList));
    }
}