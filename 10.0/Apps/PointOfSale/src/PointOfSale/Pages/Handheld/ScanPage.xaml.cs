using System;
using System.Linq;
using System.Collections.Generic;
using PointOfSale.Models;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace PointOfSale.Pages.Handheld;

public partial class ScanPage : ContentPage
{
	public ScanPage()
	{
		InitializeComponent();

       
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            // Permission check
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }

            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permission required", "Camera permission is required to scan barcodes.", "OK");
                await Shell.Current.GoToAsync("..");
                return;
            }

            // Initialize barcode options safely after view is ready
            if (barcodeView != null)
            {
                barcodeView.Options = new BarcodeReaderOptions
                {
                    Formats = BarcodeFormats.All,
                    AutoRotate = true,
                    Multiple = true
                };
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("barcodeView is null in OnAppearing");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ScanPage OnAppearing error: {ex}");
        }
    }

    protected void BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        try
        {
            foreach (var barcode in e.Results)
                Console.WriteLine($"Barcodes: {barcode.Format} -> {barcode.Value}");

            Dispatcher.DispatchAsync(async () =>
            {
                try
                {
                    var r = e.Results.FirstOrDefault();
                    if (r == null)
                        return;
                    await CloseModal(r.Value);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Dispatcher handler error: {ex}");
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"BarcodesDetected exception: {ex}");
        }
    }

    private async Task CloseModal(string value)
    {
        try
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
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"CloseModal error: {ex}");
        }
    }

    async void ImageButton_Clicked(System.Object sender, System.EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ImageButton_Clicked error: {ex}");
        }
    }
}
