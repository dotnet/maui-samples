

using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using PointOfSale.Messages;
#if IOS || MACCATALYST
using UIKit;
#endif

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
            if (view != null){
                PageGrid.Children.Remove(view);
            }
        }
	}

    void OnDragStarting(object sender, DragStartingEventArgs e)
    {
        WeakReferenceMessenger.Default.Send<DragProductMessage>(new DragProductMessage(true));

        Item item = (Item)(sender as Element).Parent.BindingContext;
        e.Data.Properties.Add("Product", item);

        var previewImage = string.Empty;
        if(item.Title == "Soda") {
            previewImage = "hunter.png";
        } else if(item.Title == "Hot Tea") {
            previewImage = "maddy.png";
        } else if(item.Title == "Milk") {
            previewImage = "sweeky.png";
        } else if(item.Title == "Coffee") {
            previewImage = "david.png";
        } else if(item.Title == "Iced Tea") {
            previewImage = "beth.png";
        } else if(item.Title == "Juice") {
            previewImage = "rachel.png";
        } else {
            return;
        }
    
    #if IOS || MACCATALYST
        Func<UIKit.UIDragPreview> action = () =>
        {
            var image = UIImage.FromBundle(previewImage);
            UIKit.UIImageView imageView = new UIKit.UIImageView(image);
            imageView.ContentMode = UIKit.UIViewContentMode.Center;
            imageView.Frame = new CoreGraphics.CGRect(0, 0, 250, 250);
            return new UIKit.UIDragPreview(imageView);
        };

        e.PlatformArgs.SetPreviewProvider(action);
    #endif
    }

    void OnDragOver(object sender, DragEventArgs e)
    {
    #if IOS || MACCATALYST
        e.PlatformArgs.SetDropProposal(new UIKit.UIDropProposal(UIKit.UIDropOperation.Copy));
    #endif
    }

    void OnDropCompleted(object sender, DropCompletedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send<DragProductMessage>(new DragProductMessage(false));
    }

    void OnDrop(object sender, DropEventArgs e)
    {
        Item product = (Item)e.Data.Properties["Product"];
        Debug.WriteLine($"{product.Title}");
        WeakReferenceMessenger.Default.Send<AddToOrderMessage>(new AddToOrderMessage(product));
        // Perform logic to take action based on retrieved value.
    }

    IDispatcherTimer timer;

    void OnPointerPressed(object sender, PointerEventArgs e)
    {
        timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(2000);
        timer.Tick += (s, e) =>
        {
            timer.Stop();
            WeakReferenceMessenger.Default.Send<AddProductMessage>(new AddProductMessage(true));
        };
    }

    void OnPointerReleased(object sender, PointerEventArgs e)
    {
        timer.Stop();
    }
}