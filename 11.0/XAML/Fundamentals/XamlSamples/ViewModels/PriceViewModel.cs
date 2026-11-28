using System.ComponentModel;

namespace XamlSamples;

// Used by ConverterCulturePage.
public class PriceViewModel : INotifyPropertyChanged
{
    double amount = 1234567.89;

    public event PropertyChangedEventHandler PropertyChanged;

    public double Amount
    {
        get => amount;
        set
        {
            if (amount != value)
            {
                amount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Amount)));
            }
        }
    }

    public DateTime Today { get; } = DateTime.Today;
}
