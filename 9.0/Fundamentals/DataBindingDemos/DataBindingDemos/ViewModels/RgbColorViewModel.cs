using System.ComponentModel;

namespace DataBindingDemos
{
    public class RgbColorViewModel : INotifyPropertyChanged
    {
        Color color;
        string name; 

        public event PropertyChangedEventHandler PropertyChanged;

        public float Red
        {
            get
            {
                return color.Red;
            }
            set
            {
                if (color.Red != value)
                {
                    Color = new Color(value, color.Green, color.Blue);
                }
            }
        }

        public float Green
        {
            get
            {
                return color.Green;
            }
            set
            {
                if (color.Green != value)
                {
                    Color = new Color(color.Red, value, color.Blue);
                }
            }
        }

        public float Blue
        {
            get
            {
                return color.Blue;
            }
            set
            {
                if (color.Blue != value)
                {
                    Color = new Color(color.Red, color.Green, value);
                }
            }
        }

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                if (color != value)
                {
                    color = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Red"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Green"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Blue"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Color"));

                    Name = NamedColor.GetNearestColorName(color);
                }
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            private set
            {
                if (name != value)
                {
                    name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }
    }
}
