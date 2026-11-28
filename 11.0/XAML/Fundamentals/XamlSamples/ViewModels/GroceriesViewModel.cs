using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace XamlSamples;

// Used by AncestorBindingsPage.
public class GroceriesViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public GroceriesViewModel()
    {
        AddCommand = new Command<GroceryItem>(item =>
        {
            item.Count++;
            OnTotalChanged();
        });

        ClearCommand = new Command(
            () =>
            {
                foreach (var item in Items)
                    item.Count = 0;
                OnTotalChanged();
            },
            () => TotalCount > 0);
    }

    public IReadOnlyList<GroceryItem> Items { get; } =
    [
        new("Apples", "red_apple.png"),
        new("Avocados", "avocado.png"),
        new("Baguettes", "baguette_bread.png"),
        new("Carrots", "carrot.png"),
        new("Cheese", "cheese_wedge.png"),
        new("Coffee", "hot_beverage.png"),
    ];

    public int TotalCount => Items.Sum(item => item.Count);

    public string Summary => TotalCount switch
    {
        0 => "Your basket is empty",
        1 => "1 item in your basket",
        _ => $"{TotalCount} items in your basket"
    };

    public ICommand AddCommand { get; }

    public ICommand ClearCommand { get; }

    void OnTotalChanged()
    {
        OnPropertyChanged(nameof(TotalCount));
        OnPropertyChanged(nameof(Summary));
        ((Command)ClearCommand).ChangeCanExecute();
    }

    void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class GroceryItem(string name, string image) : INotifyPropertyChanged
{
    int count;

    public event PropertyChangedEventHandler PropertyChanged;

    public string Name { get; } = name;

    public string Image { get; } = image;

    public int Count
    {
        get => count;
        set
        {
            if (count != value)
            {
                count = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
            }
        }
    }
}
