namespace WorkingWithFiles;

/// <summary>
/// This page includes input boxes and buttons that allow the text to be
/// saved-to and loaded-from a file.
/// </summary>
public partial class SaveAndLoadTextPage : ContentPage
{
    private readonly string _fileName = Path.Combine(FileSystem.AppDataDirectory, "temp.txt");

    public SaveAndLoadTextPage()
    {
        InitializeComponent();
        LoadButton.IsEnabled = File.Exists(_fileName);
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        try
        {
            SaveButton.IsEnabled = false;
            LoadButton.IsEnabled = false;

            await File.WriteAllTextAsync(_fileName, InputEntry.Text);
            
            await DisplayAlert("Success", "Text saved successfully!", "OK");
            LoadButton.IsEnabled = true;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error saving file: {ex.Message}", "OK");
        }
        finally
        {
            SaveButton.IsEnabled = true;
            LoadButton.IsEnabled = File.Exists(_fileName);
        }
    }

    private async void OnLoadClicked(object sender, EventArgs e)
    {
        try
        {
            SaveButton.IsEnabled = false;
            LoadButton.IsEnabled = false;

            if (File.Exists(_fileName))
            {
                var content = await File.ReadAllTextAsync(_fileName);
                OutputLabel.Text = content;
            }
            else
            {
                await DisplayAlert("Error", "No saved file found.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error loading file: {ex.Message}", "OK");
        }
        finally
        {
            SaveButton.IsEnabled = true;
            LoadButton.IsEnabled = File.Exists(_fileName);
        }
    }
}
