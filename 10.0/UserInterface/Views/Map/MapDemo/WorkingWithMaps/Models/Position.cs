using System.ComponentModel;

namespace WorkingWithMaps.Models
{
    public class Position : INotifyPropertyChanged
    {
        Location _location;

        public string Address { get; }
        public string Description { get; }

        public Location Location
        {
            get => _location;
            set
            {
                if (Location == null || !_location.Equals(value))
                {
                    _location = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Location)));
                }
            }
        }

        public Position(string address, string description, Location location)
        {
            Address = address;
            Description = description;
            Location = location;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}