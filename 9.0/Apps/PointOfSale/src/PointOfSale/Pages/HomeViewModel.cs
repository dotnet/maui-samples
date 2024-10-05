using System;
using PointOfSale.Messages;

namespace PointOfSale.Pages;

public partial class HomeViewModel : ObservableObject
{
    [ObservableProperty]
    ObservableCollection<Item> _products;

    [ObservableProperty]
    string category = ItemCategory.Noodles.ToString();

    partial void OnCategoryChanged(string value)
    {
       ItemCategory category = (ItemCategory)Enum.Parse(typeof(ItemCategory), value);
       Products = new ObservableCollection<Item>(
           AppData.Items.Where(x => x.Category == category).ToList()
       );
       OnPropertyChanged(nameof(Products));
    }

    public HomeViewModel()
    {
        Products = new ObservableCollection<Item>(
            AppData.Items.Where(x=>x.Category == ItemCategory.Noodles).ToList()
        );
    }

    [RelayCommand]
    async Task Preferences()
    {
        await Shell.Current.GoToAsync($"{nameof(SettingsPage)}?sub=appearance");
    }

    [RelayCommand]
    void AddProduct()
    {
        WeakReferenceMessenger.Default.Send<AddProductMessage>(new AddProductMessage(true));
    }
}