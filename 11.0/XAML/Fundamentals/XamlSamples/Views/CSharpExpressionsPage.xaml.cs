namespace XamlSamples;

public partial class CSharpExpressionsPage : ContentPage
{
    public CSharpExpressionsPage()
    {
        InitializeComponent();
    }

    // The Clicked lambdas in the XAML run in this class, so they reach the view model through this property.
    OrderViewModel Order => (OrderViewModel)BindingContext;
}
