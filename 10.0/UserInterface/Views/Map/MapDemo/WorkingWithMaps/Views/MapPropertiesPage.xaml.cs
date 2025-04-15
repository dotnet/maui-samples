using System.Diagnostics;
using Microsoft.Maui.Controls.Maps;

namespace WorkingWithMaps.Views;

public partial class MapPropertiesPage : ContentPage
{
    public MapPropertiesPage()
    {
        InitializeComponent();

        scrollEnabledCheckBox.CheckedChanged += OnCheckBoxCheckedChanged;
        zoomEnabledCheckBox.CheckedChanged += OnCheckBoxCheckedChanged;
    }

    void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        Debug.WriteLine($"MapClick: {e.Location.Latitude}, {e.Location.Longitude}");
    }

    void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        CheckBox checkBox = sender as CheckBox;

        switch (checkBox.StyleId)
        {
            case "scrollEnabledCheckBox":
                map.IsScrollEnabled = !map.IsScrollEnabled;
                break;
            case "zoomEnabledCheckBox":
                map.IsZoomEnabled = !map.IsZoomEnabled;
                break;
            case "showUserCheckBox":
                map.IsShowingUser = !map.IsShowingUser;
                break;
            case "showTrafficCheckBox":
                map.IsTrafficEnabled = !map.IsTrafficEnabled;
                break;
        }
    }
}
