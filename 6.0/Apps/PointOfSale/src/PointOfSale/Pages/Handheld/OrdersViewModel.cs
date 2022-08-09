namespace PointOfSale.Pages.Handheld;

[INotifyPropertyChanged]
public partial class OrdersViewModel
{
    [ObservableProperty]
    private ObservableCollection<Order> _orders;

    public OrdersViewModel()
    {
        _orders = new ObservableCollection<Order>(AppData.Orders);
    }
}