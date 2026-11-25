using System.Security.Cryptography;
using Plugin.Maui.KeyListener;
using PointOfSale.Messages;

namespace PointOfSale.Pages;

public partial class AddProductView : ContentView, IDisposable
{
	KeyboardBehavior kb = new KeyboardBehavior();
	public AddProductView()
	{
		InitializeComponent();

		this.Behaviors.Add(kb);

        kb.KeyUp += Kb_KeyUp;
	}

    private void Kb_KeyUp(object sender, KeyPressedEventArgs e)
    {
        if(e.Keys == KeyboardKeys.Escape)
		{
			(this.BindingContext as AddProductViewModel).CancelCommand.Execute(null);
		}
    }

    public void Dispose()
    {
        kb.KeyUp -= Kb_KeyUp;
		this.Behaviors.Remove(kb);
    }
}
