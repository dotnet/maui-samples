using System;
namespace PointOfSale.Pages.Views;

[INotifyPropertyChanged]
public partial class OrderCartViewModel
{
    [ObservableProperty]
    Order order;

    int index = 0;

    public OrderCartViewModel()
    {
        Order = AppData.Orders.First();
    }

    [RelayCommand]
    async Task PlaceOrder()
    {
        await App.Current.MainPage.DisplayAlert("Not Implemented", "Wouldn't it be cool tho?", "Okay");
        if(index < (AppData.Orders.Count - 1))
            index++;
        else
            index = 0;
        Order = AppData.Orders[index];
    }
}