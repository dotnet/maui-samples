namespace PointOfSale.Models;

public partial class Item : ObservableObject
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