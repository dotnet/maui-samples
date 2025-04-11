using System.ComponentModel;

namespace ControlGallery.Models
{
    // Used in TabbedPageDemoPage, CarouselPageDemoPage & FlyoutPageDemoPage.
    public class NamedColor
    {
        Color _color;
        string _name;
        float _hue;
        float _saturation;
        float _luminosity;

        public event PropertyChangedEventHandler PropertyChanged;

        public float Red
        {
            get
            {
                return _color.Red;
            }
        }

        public float Green
        {
            get
            {
                return _color.Blue;
            }
        }

        public float Blue
        {
            get
            {
                return _color.Blue;
            }
        }

        public float Hue
        {
            get
            {
                return _hue;
            }
            set
            {
                if (_hue != value)
                {
                    Color = Color.FromHsla(value, _saturation, _luminosity);
                }
            }
        }

        public float Saturation
        {
            get
            {
                return _saturation;
            }
            set
            {
                if (_saturation != value)
                {
                    Color = Color.FromHsla(_hue, value, _luminosity);
                }
            }
        }

        public float Luminosity
        {
            get
            {
                return _luminosity;
            }
            set
            {
                if (_luminosity != value)
                {
                    Color = Color.FromHsla(_hue, _saturation, value);
                }
            }
        }

        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    _hue = _color.GetHue();
                    _saturation = _color.GetSaturation();
                    _luminosity = _color.GetLuminosity();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Hue"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Saturation"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Luminosity"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Color"));
                }
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }


        public NamedColor()
        {
        }

        public NamedColor(string name, Color color)
        {
            Name = name;
            Color = color;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
 