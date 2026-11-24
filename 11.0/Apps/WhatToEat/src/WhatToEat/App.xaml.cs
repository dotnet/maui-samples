using Recipes.Services;

namespace Recipes;

public partial class App : Microsoft.Maui.Controls.Application
{

    public App()
    {
			InitializeComponent();
        DependencyService.Register<MockDataStore>();
		}

    protected override Window CreateWindow(IActivationState activationState)
        => new Window(new AppShell());
}
