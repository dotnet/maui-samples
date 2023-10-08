namespace Animations.Controls;

public class PickerCell : ViewCell
{

    private Label _label { get; set; }
    private View _picker { get; set; }
    private StackLayout _layout { get; set; }

    public string Label
    {
        get
        {
            return _label.Text;
        }
        set
        {
            _label.Text = value;
        }
    }

    public View Picker
    {
        set
        {
            //Remove picker if it exists
            if (_picker != null)
            {
                _layout.Children.Remove(_picker);
            }

            //Set its value
            _picker = value;
            //Add to layout
            _layout.Children.Add(_picker);

        }
    }

    public PickerCell()
    {

        _label = new Label()
        {
            VerticalOptions = LayoutOptions.Center,
            FontSize = 15
        };
        _layout = new StackLayout()
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Center,
            Padding = new Thickness(15, 5),
            Children =
            {
                _label
            }
        };

        this.View = _layout;

    }

}