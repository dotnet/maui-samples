namespace PointOfSale.Pages;

public partial class DashboardViewModel : ObservableObject
{
	[RelayCommand]
	async Task ViewAll()
	{
		await App.Current.Windows[0].Page.DisplayAlert("Not Implemented", "Wouldn't it be nice tho?", "Okay");
	}
}
