using System;
namespace PointOfSale.Pages.Views;

public partial class OrderCartViewModel : ObservableObject
{
    [ObservableProperty]
    Order order;

    [ObservableProperty]
    ObservableCollection<Item> items;

    int index = 0;

    public OrderCartViewModel()
    {
        Order = AppData.Orders.First();
        Items = new ObservableCollection<Item>(Order.Items);

        WeakReferenceMessenger.Default.Register<AddToOrderMessage>(this, (r, m) =>
        {
            AddToOrder(m.Value);
            Items = new ObservableCollection<Item>(Order.Items);
            OnPropertyChanged(nameof(Items));
        });
    }

    private void AddToOrder(Item item)
    {
        //if item is in the order alread,  increment the quantity
        var existing = Order.Items.Where(x => x.Title == item.Title).SingleOrDefault();
        if (existing != null)
        {
            existing.Quantity++;
        }
        else
        {
            Order.Items.Add(item);
        }
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