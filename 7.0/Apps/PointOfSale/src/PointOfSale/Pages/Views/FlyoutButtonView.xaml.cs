namespace PointOfSale.Pages.Views;

public partial class FlyoutButtonView : RadioButton
{
	public FlyoutButtonView()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(IconProperty), typeof(string), typeof(FlyoutButtonView), string.Empty);

    public string Icon
    {
        get => (string)GetValue(FlyoutButtonView.IconProperty);
        set => SetValue(FlyoutButtonView.IconProperty, value);
    }
}
