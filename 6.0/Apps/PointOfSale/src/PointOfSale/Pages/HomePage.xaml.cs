namespace PointOfSale.Pages;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();

        MessagingCenter.Subscribe<HomeViewModel, string>(this, "action", async (sender, arg) =>
        {
			NavSubContent(arg);
        });

        MessagingCenter.Subscribe<AddProductViewModel, string>(this, "action", async (sender, arg) =>
        {
            NavSubContent(arg);
        });
    }

    void MenuFlyoutItem_ParentChanged(System.Object sender, System.EventArgs e)
    {
		if (sender is BindableObject bo)
			bo.BindingContext = this.BindingContext;
    }

	

	public void NavSubContent(string sub)
	{
        var displayWidth = DeviceDisplay.Current.MainDisplayInfo.Width;
        
        if (sub.ToLower() == "add")
		{
			var addForm = new AddProductView();
			PageGrid.Add(addForm, 1);
			Grid.SetRowSpan(addForm, 3);
			// translate off screen right
			addForm.TranslationX = displayWidth - addForm.X;
			addForm.TranslateTo(0, 0, 800, easing: Easing.CubicOut);
		}
		else if(sub.ToLower() == "done")
		{
			// remove the product window

			var view = (AddProductView)PageGrid.Children.Where(v => v.GetType() == typeof(AddProductView)).SingleOrDefault();

            var x = DeviceDisplay.Current.MainDisplayInfo.Width;
            view.TranslateTo(displayWidth - view.X, 0, 800, easing: Easing.CubicIn);

            if (view != null)
				PageGrid.Children.Remove(view);

        }
	}
}