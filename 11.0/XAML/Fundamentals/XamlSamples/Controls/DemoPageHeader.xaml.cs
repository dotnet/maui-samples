namespace XamlSamples;

public partial class DemoPageHeader : ContentView
{
    // DemoPageTemplate binds this to the page that the template is applied to.
    public static readonly BindableProperty PageProperty =
        BindableProperty.Create(nameof(Page), typeof(Page), typeof(DemoPageHeader),
            propertyChanged: (bindable, oldValue, newValue) =>
                ((DemoPageHeader)bindable).BindingContext = newValue is Page page ? PageDataViewModel.Find(page.GetType()) : null);

    public DemoPageHeader()
    {
        InitializeComponent();
    }

    public Page Page
    {
        get => (Page)GetValue(PageProperty);
        set => SetValue(PageProperty, value);
    }

    async void OnDocsClicked(object sender, EventArgs e)
    {
        if (BindingContext is PageDataViewModel pageData)
        {
            await Launcher.Default.OpenAsync(pageData.DocsUrl);
        }
    }
}
