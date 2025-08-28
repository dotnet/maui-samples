using System.Reflection;
using System.Xml.Serialization;
using WorkingWithFiles.Models;

namespace WorkingWithFiles;

public partial class LoadResourceXmlPage : ContentPage
{
    public LoadResourceXmlPage()
    {
        InitializeComponent();
        LoadXmlResource();
    }

    private async void LoadXmlResource()
    {
        try
        {
            // Load embedded resource XML file
            var assembly = typeof(LoadResourceXmlPage).GetTypeInfo().Assembly;
            using var stream = assembly.GetManifestResourceStream("WorkingWithFiles.Resources.Data.LibXmlResource.xml");
            
            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                var serializer = new XmlSerializer(typeof(List<Monkey>));
                var monkeys = (List<Monkey>?)serializer.Deserialize(reader);
                
                MonkeysCollectionView.ItemsSource = monkeys;
            }
            else
            {
                await DisplayAlertAsync("Error", "Could not load XML resource", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Error loading XML resource: {ex.Message}", "OK");
        }
    }
}
