

using PointOfSale.Messages;

namespace PointOfSale.Pages;

public partial class HomePage : ContentPage
{
    public HomePage()
	{
		InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        WeakReferenceMessenger.Default.Register<AddProductMessage>(this, (r, m) =>
        {
            NavSubContent(m.Value);
        });
    }

    void MenuFlyoutItem_ParentChanged(System.Object sender, System.EventArgs e)
    {
		if (sender is BindableObject bo)
			bo.BindingContext = this.BindingContext;
    }

	

	public async void NavSubContent(bool show)
	{
        var displayWidth = DeviceDisplay.Current.MainDisplayInfo.Width;
        
        if (show)
		{
			var addForm = new AddProductView();
			PageGrid.Add(addForm, 1);
			Grid.SetRowSpan(addForm, 3);
			// translate off screen right
			addForm.TranslationX = displayWidth - addForm.X;
			_ = addForm.TranslateTo(0, 0, 800, easing: Easing.CubicOut);

            _ = BlockScreen.FadeTo(0.8, 800, easing: Easing.CubicOut);
            BlockScreen.InputTransparent = false;

        }
		else
		{
			// remove the product window

			var view = (AddProductView)PageGrid.Children.Where(v => v.GetType() == typeof(AddProductView)).SingleOrDefault();

            var x = DeviceDisplay.Current.MainDisplayInfo.Width;
            _ = view.TranslateTo(displayWidth - view.X, 0, 800, easing: Easing.CubicIn);

            _ = BlockScreen.FadeTo(0, 800, easing: Easing.CubicOut);
            BlockScreen.InputTransparent = true;

            await Task.Delay(800);
            if (view != null)
                PageGrid.Children.Remove(view);
        }
	}
}