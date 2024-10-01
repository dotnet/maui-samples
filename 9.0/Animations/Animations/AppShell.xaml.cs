namespace Animations;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(EasingEditorPage), typeof(EasingEditorPage));
    }
}
