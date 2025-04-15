using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WorkingWithMaps.Models;

namespace WorkingWithMaps.ViewModels;

public class PinItemsSourcePageViewModel
{
    int _pinCreatedCount = 0;
    readonly ObservableCollection<Position> _positions;

    public IEnumerable Positions => _positions;

    public ICommand AddLocationCommand { get; }
    public ICommand RemoveLocationCommand { get; }
    public ICommand ClearLocationsCommand { get; }
    public ICommand UpdateLocationsCommand { get; }
    public ICommand ReplaceLocationCommand { get; }

    public PinItemsSourcePageViewModel()
    {
        _positions = new ObservableCollection<Position>()
        {
            new Position("New York, USA", "The City That Never Sleeps", new Location(40.67, -73.94)),
            new Position("Los Angeles, USA", "City of Angels", new Location(34.11, -118.41)),
            new Position("San Francisco, USA", "Bay City", new Location(37.77, -122.45))
        };

        AddLocationCommand = new Command(AddLocation);
        RemoveLocationCommand = new Command(RemoveLocation);
        ClearLocationsCommand = new Command(() => _positions.Clear());
        UpdateLocationsCommand = new Command(UpdateLocations);
        ReplaceLocationCommand = new Command(ReplaceLocation);
    }

    void AddLocation()
    {
        _positions.Add(NewPosition());
    }

    void RemoveLocation()
    {
        if (_positions.Any())
        {
            _positions.Remove(_positions.First());
        }
    }

    void UpdateLocations()
    {
        if (!_positions.Any())
        {
            return;
        }

        double lastLatitude = _positions.Last().Location.Latitude;
        foreach (Position position in Positions)
        {
            position.Location = new Location(lastLatitude, position.Location.Longitude);
        }
    }

    void ReplaceLocation()
    {
        if (!_positions.Any())
        {
            return;
        }

        _positions[_positions.Count - 1] = NewPosition();
    }

    Position NewPosition()
    {
        _pinCreatedCount++;
        return new Position(
            $"Pin {_pinCreatedCount}",
            $"Desc {_pinCreatedCount}",
            RandomPosition.Next(new Location(39.8283459, -98.5794797), 8, 19));
    }
}
