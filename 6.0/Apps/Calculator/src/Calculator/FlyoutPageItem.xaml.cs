namespace Calculator;

public partial class FlyoutPageItem : ContentPage
{
    public Type TitleType { get; set; }
    public Type TargetType { get; set; }

    public FlyoutPageItem()
    {
        InitializeComponent();
    }
}
