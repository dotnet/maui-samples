using System;
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
}

