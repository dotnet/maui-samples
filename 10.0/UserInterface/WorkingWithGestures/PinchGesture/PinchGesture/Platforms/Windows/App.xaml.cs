using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PinchGesture.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class for WinUI.
    /// This namespace matches the x:Class in Platforms/Windows/App.xaml so the partial class
    /// generated from XAML merges with this code-behind without conflicting with the MAUI `App` type.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
