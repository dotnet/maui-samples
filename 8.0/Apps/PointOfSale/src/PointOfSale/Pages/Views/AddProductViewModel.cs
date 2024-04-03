using System;
using PointOfSale.Messages;

namespace PointOfSale.Pages;

public partial class AddProductViewModel : ObservableObject
{
    [ObservableProperty]
    Item item = new Item();

    [ObservableProperty]
    string category = ItemCategory.Noodles.ToString();

    [ObservableProperty]
    string imagePath = "noimage.png";

    [ObservableProperty]
    ImageSource image;

    [RelayCommand]
    void Save()
    {
        ItemCategory cat = (ItemCategory)Enum.Parse(typeof(ItemCategory), Category);
        Item.Category = cat;
        AppData.Items.Add(Item);

        WeakReferenceMessenger.Default.Send<AddProductMessage>(new AddProductMessage(false));
    }

    [RelayCommand]
    Task Cancel()
    {
        WeakReferenceMessenger.Default.Send<AddProductMessage>(new AddProductMessage(false));

        return Task.CompletedTask;
    }

    [RelayCommand]
    async Task ChangeImage()
    {
        PickOptions options = new()
        {
            PickerTitle = "Please select a photo file"
        };

        var file = await PickAndShow(options);
        Item.Image = ImagePath = file.FullPath;
    }

    public async Task<FileResult> PickAndShow(PickOptions options)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                {
                    using var stream = await result.OpenReadAsync();
                    Image = ImageSource.FromStream(() => stream);
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            // The user canceled or something went wrong
            Console.WriteLine(ex);
        }

        return null;
    }
}

