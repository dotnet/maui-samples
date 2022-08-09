using System;
namespace PointOfSale.Pages;

[INotifyPropertyChanged]
public partial class HomeViewModel
{
    [ObservableProperty]
    ObservableCollection<Item> _products;

    [ObservableProperty]
    string category = ItemCategory.Noodles.ToString();

    partial void OnCategoryChanged(string cat)
    {
       ItemCategory category = (ItemCategory)Enum.Parse(typeof(ItemCategory), cat);
       _products = new ObservableCollection<Item>(
           AppData.Items.Where(x => x.Category == category).ToList()
       );
       OnPropertyChanged(nameof(Products));
    }

    public HomeViewModel()
    {
        _products = new ObservableCollection<Item>(
            AppData.Items.Where(x=>x.Category == ItemCategory.Noodles).ToList()
        );
    }

    [RelayCommand]
    async Task Preferences()
    {
        await Shell.Current.GoToAsync($"{nameof(SettingsPage)}?sub=appearance");
    }

    [RelayCommand]
    async Task AddProduct()
    {
        MessagingCenter.Send<HomeViewModel, string>(this, "action", "add");
    }
}