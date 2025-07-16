using System.Reflection;

namespace WorkingWithFiles;

public partial class LoadResourceTextPage : ContentPage
{
    public LoadResourceTextPage()
    {
        InitializeComponent();
        LoadTextResource();
    }

    private async void LoadTextResource()
    {
        try
        {
            // Load embedded resource text file
            var assembly = typeof(LoadResourceTextPage).GetTypeInfo().Assembly;
            using var stream = assembly.GetManifestResourceStream("WorkingWithFiles.Resources.Data.LibTextResource.txt");
            
            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                var text = await reader.ReadToEndAsync();
                TextLabel.Text = text;
            }
            else
            {
                TextLabel.Text = "Error: Could not load text resource";
            }
        }
        catch (Exception ex)
        {
            TextLabel.Text = $"Error loading text resource: {ex.Message}";
        }
    }
}
