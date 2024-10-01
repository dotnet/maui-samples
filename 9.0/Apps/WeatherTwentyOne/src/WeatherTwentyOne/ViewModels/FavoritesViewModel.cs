using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WeatherClient2021;
using Location = WeatherClient2021.Location;

namespace WeatherTwentyOne.ViewModels;

public class FavoritesViewModel : INotifyPropertyChanged
{
    IWeatherService weatherService = new WeatherService(null);

    private ObservableCollection<Location> favorites;
    public ObservableCollection<Location> Favorites {
        get {
            return favorites;
        }

        set {
            favorites = value;
            OnPropertyChanged();
        }
    }

    async void Fetch()
    {
        var locations = await weatherService.GetLocations(string.Empty);

        UpdateFavorites(locations);

        OnPropertyChanged(nameof(Favorites));

    }

    private void UpdateFavorites(IEnumerable<Location> locations)
    {
        favorites = new ObservableCollection<Location>();
        for (int i = locations.Count() - 1; i >= 0; i--)
        {
            favorites.Add(locations.ElementAt(i));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null)
            handler(this, new PropertyChangedEventArgs(propertyName));
    }

    public FavoritesViewModel()
    {
        Fetch();
    }
}
