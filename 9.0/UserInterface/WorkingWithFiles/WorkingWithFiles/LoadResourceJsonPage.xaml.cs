using System.Reflection;
using System.Text.Json;
using WorkingWithFiles.Models;

namespace WorkingWithFiles;

public partial class LoadResourceJsonPage : ContentPage
{
    public LoadResourceJsonPage()
    {
        InitializeComponent();
        LoadJsonResource();
    }

    private async void LoadJsonResource()
    {
        try
        {
            // Load embedded resource JSON file
            var assembly = typeof(LoadResourceJsonPage).GetTypeInfo().Assembly;
            using var stream = assembly.GetManifestResourceStream("WorkingWithFiles.Resources.Data.LibJsonResource.json");
            
            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var rootObject = JsonSerializer.Deserialize<Rootobject>(json, options);
                
                EarthquakesCollectionView.ItemsSource = rootObject?.earthquakes;
            }
            else
            {
                await DisplayAlert("Error", "Could not load JSON resource", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error loading JSON resource: {ex.Message}", "OK");
        }
    }
}
