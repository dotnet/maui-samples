﻿namespace PointOfSale.Pages;

public partial class SettingsViewModel : ObservableObject
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
        await App.Current.Windows[0].Page.DisplayAlert("Not Implemented", "Wouldn't it be nice tho?", "Okay");
    }
}
