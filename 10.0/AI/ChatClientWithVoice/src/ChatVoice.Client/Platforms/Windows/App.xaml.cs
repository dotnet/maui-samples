using Microsoft.UI.Xaml;

namespace ChatVoice.Client.WinUI;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        this.InitializeComponent();
    }

    protected override MauiApp CreateMauiApp() => ChatVoice.Client.MauiProgram.CreateMauiApp();
}
