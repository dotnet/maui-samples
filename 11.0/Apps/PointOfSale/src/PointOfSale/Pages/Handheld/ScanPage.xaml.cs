using PointOfSale.Models;
using ZXing.Net.Maui;

namespace PointOfSale.Pages.Handheld;

public partial class ScanPage : ContentPage
{
	public ScanPage()
	{
		InitializeComponent();
        barcodeView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.All,
            AutoRotate = true,
            Multiple = true
        };
    }

    protected void BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        foreach (var barcode in e.Results)
            Console.WriteLine($"Barcodes: {barcode.Format} -> {barcode.Value}");

        Dispatcher.DispatchAsync(async () =>
        {
            var r = e.Results.First();

            await CloseModal(r.Value);
        });
    }

    private async Task CloseModal(string value)
    {
        var item = new Item
        {
            Price = 0,
            Quantity = 1,
            Title = value
        };
        var navigationParameter = new Dictionary<string, object>
        {
                { "Item", item }
            };

        await Shell.Current.GoToAsync("..", navigationParameter);
    }

    async void ImageButton_Clicked(System.Object sender, System.EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
