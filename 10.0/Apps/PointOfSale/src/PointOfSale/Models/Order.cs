using CommunityToolkit.Mvvm.ComponentModel;
using PointOfSale.Pages.Handheld;

namespace PointOfSale.Models;

public partial class Order : ObservableObject
{
    [ObservableProperty]
    private int table;

    [ObservableProperty]
    private byte[] signatureData;

    [ObservableProperty] 
    private double tip;

    public string Total
    {
        get
        {
            var tot = Items.Sum(i => (i.Price * i.Quantity));
            if (Tip != 0)
                tot = tot + (tot * Tip);
            return tot.ToString("N2");
        }
    }

    [ObservableProperty]
    private List<Item> items;

    [ObservableProperty] 
    private string status;

    [ObservableProperty]
    private OrderType orderType = OrderType.DineIn;
    
    private static readonly Random _random = new Random();
    
    private static readonly string[] brushes = new string[] { "#FFB572", "#65B0F6", "#FF7CA3", "#50D1AA", "#9290FE" };
    public static Brush RandomBrush
    {
        get
        {
            var id = _random.Next(0, 4);
            return new SolidColorBrush(Color.Parse(brushes[id]));
        }
    }

    [RelayCommand]
    private async Task Pay()
    {
        try
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "Order", this }
            };
            await Shell.Current.GoToAsync($"{nameof(OrderDetailsPage)}", navigationParameter);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
}