namespace PointOfSale.Pages;

[INotifyPropertyChanged]
public partial class SettingsViewModel
{
    [ObservableProperty]
    ObservableCollection<Item> _products;

    public SettingsViewModel()
    {
        _products = new ObservableCollection<Item>(
            AppData.Items
        );
    }

    [RelayCommand]
    async Task NotImplemented()
    {
        await App.Current.MainPage.DisplayAlert("Not Implemented", "Wouldn't it be nice tho?", "Okay");
    }
}

