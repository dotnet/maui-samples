using CommunityToolkit.Mvvm.Messaging;
using PointOfSale.Messages;

namespace PointOfSale.Pages.Handheld;

[QueryProperty("Order","Order")]
public partial class SignatureViewModel : ObservableObject
{
    [ObservableProperty]
    Order order;
    
    [RelayCommand]
    async Task Done()
    {
        WeakReferenceMessenger.Default.Send<SaveSignatureMessage>(
            new SaveSignatureMessage(Order.Table)
        );

        var navigationParameter = new Dictionary<string, object>
        {
            { "Order", Order }
        };
        await Shell.Current.GoToAsync($"{nameof(ReceiptPage)}", navigationParameter);
    }

    [RelayCommand]
    Task Clear()
    {
        WeakReferenceMessenger.Default.Send<ClearSignatureMessage>(
            new ClearSignatureMessage(true)
        );

        return Task.CompletedTask;
    }
}