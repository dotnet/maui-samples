using System.Reflection;
using System.Windows.Input;


namespace NavigationPageTitleView;

public partial class MainPage : ContentPage
{
    Page? _originalRoot;

    public ICommand NavigateCommand { get; private set; }


    public MainPage()
    {
        InitializeComponent();

        NavigateCommand = new Command<Type>(async (pageType) => await NavigateToPage(pageType));
        BindingContext = this;
    }

    async Task NavigateToPage(Type pageType)
    {
        Type[] types = new Type[] { typeof(Command) };
        ConstructorInfo? info = pageType.GetConstructor(types);
        if (info != null)
        {
            Page? page = Activator.CreateInstance(pageType, new Command(RestoreOriginal)) as Page;
            if (page is iOSExtendedTitleViewPage)
            {
                page = new iOSNavigationPage(page);
            }
            else if (page is AndroidExtendedTitleViewPage)
            {
                page = new AndroidNavigationPage(page);
            }
            if (page != null)
                SetRoot(page);
        }
        else
        {
            Page? page = Activator.CreateInstance(pageType) as Page;
            if (page != null)
                await Navigation.PushAsync(page);
        }
    }

    void SetRoot(Page page)
    {
        var app = Application.Current as App;
        if (app == null)
        {
            return;
        }

        // Use Windows[0].Page instead of deprecated MainPage
        _originalRoot = app.Windows.Count > 0 ? app.Windows[0].Page : null;
        app.SetMainPage(page);
    }

    void RestoreOriginal()
    {
        if (_originalRoot == null)
        {
            return;
        }

        var app = Application.Current as App;
        if (app != null)
        {
            app.SetMainPage(_originalRoot);
        }
    }
}
