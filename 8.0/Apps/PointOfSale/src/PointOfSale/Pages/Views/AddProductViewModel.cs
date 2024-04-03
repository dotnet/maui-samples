using System;
using PointOfSale.Messages;

namespace PointOfSale.Pages;

[INotifyPropertyChanged]
public partial class AddProductViewModel
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
        ItemCategory cat = (ItemCategory)Enum.Parse(typeof(ItemCategory), category);
        item.Category = cat;
        AppData.Items.Add(item);

        WeakReferenceMessenger.Default.Send<AddProductMessage>(new AddProductMessage(false));
    }

    [RelayCommand]
    void Cancel()
    {
        WeakReferenceMessenger.Default.Send<AddProductMessage>(new AddProductMessage(false));
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
        }

        return null;
    }
}

