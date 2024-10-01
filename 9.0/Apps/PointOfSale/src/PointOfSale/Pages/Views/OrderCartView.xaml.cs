using PointOfSale.Messages;

namespace PointOfSale.Pages.Views;

public partial class OrderCartView : ContentView
{
	public OrderCartView()
	{
		InitializeComponent();

		WeakReferenceMessenger.Default.Register<DragProductMessage>(this, (r, m) =>
        {
            if(m.Value)
				VisualStateManager.GoToState(this, "AcceptDrop");
			else
				VisualStateManager.GoToState(this, "Normal");
        });
	}
}
