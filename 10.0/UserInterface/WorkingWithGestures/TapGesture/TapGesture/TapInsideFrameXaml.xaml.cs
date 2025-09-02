namespace TapGesture
{	
	public partial class TapInsideFrameXaml : ContentPage
	{	
		public TapInsideFrameXaml()
		{
			InitializeComponent();

			// The TapViewModel contains the TapCommand which is wired up in Xaml
			BindingContext = new TapViewModel();
		}
	}
}

