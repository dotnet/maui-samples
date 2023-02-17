namespace PointOfSale.Pages;

[INotifyPropertyChanged]
public partial class DashboardViewModel
{
	[RelayCommand]
	async Task ViewAll()
	{
		await App.Current.MainPage.DisplayAlert("Not Implemented", "Wouldn't it be nice tho?", "Okay");
	}
}

