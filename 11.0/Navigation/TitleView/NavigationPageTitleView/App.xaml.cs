namespace NavigationPageTitleView;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }
    public void SetMainPage(Page page)
    {
        if (Windows.Count > 0)
        {
            Windows[0].Page = page;
        }
    }
    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new NavigationPage(new MainPage()));
    }
}
