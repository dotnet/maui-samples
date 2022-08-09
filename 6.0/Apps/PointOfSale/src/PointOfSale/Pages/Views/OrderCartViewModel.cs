using System;
namespace PointOfSale.Pages.Views;

[INotifyPropertyChanged]
public partial class OrderCartViewModel
{
    [ObservableProperty]
    Order order;

    public OrderCartViewModel()
    {
        Order = AppData.Orders.First();
    }
}