using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XamlSamples;

// Used by CSharpExpressionsPage. The page computes everything it shows from these properties.
public class OrderViewModel : INotifyPropertyChanged
{
    string firstName = "Ada";
    string lastName = "Lovelace";
    int quantity = 3;

    public event PropertyChangedEventHandler PropertyChanged;

    public string FirstName
    {
        get => firstName;
        set => SetProperty(ref firstName, value);
    }

    public string LastName
    {
        get => lastName;
        set => SetProperty(ref lastName, value);
    }

    public int Quantity
    {
        get => quantity;
        set => SetProperty(ref quantity, Math.Max(0, value));
    }

    public decimal Price { get; } = 12.50m;

    void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return;

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
