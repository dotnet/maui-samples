namespace PointOfSale.Models;

[INotifyPropertyChanged]
public partial class Item
{
    [ObservableProperty]
    string title;

    [ObservableProperty]
    int quantity;

    [ObservableProperty]
    string image;

    [ObservableProperty]
    double price;

    partial void OnQuantityChanged(int value)
    {
        OnPropertyChanged(nameof(SubTotal));
    }

    public ItemCategory Category { get; set; }

    public double SubTotal {
        get
        {
            return Price * Quantity;
        }
    }
}