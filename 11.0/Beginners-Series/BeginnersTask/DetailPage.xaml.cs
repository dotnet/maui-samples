using BeginnersTasks.ViewModel;

namespace BeginnersTasks;

public partial class DetailPage : ContentPage
{
	public DetailPage(DetailViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}