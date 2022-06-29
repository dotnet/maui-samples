using GridDemos.Converters;

namespace GridDemos.Views.Code
{
    public class ColorSlidersGridPage : ContentPage
    {
        BoxView boxView;
        Slider redSlider;
        Slider greenSlider;
        Slider blueSlider;

        public ColorSlidersGridPage()
        {
            // Create an implicit style for the Labels
            Style labelStyle = new Style(typeof(Label))
            {
                Setters =
                {
                    new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center }
                }
            };
            Resources.Add(labelStyle);

            // Root page layout
            Grid rootGrid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = 250 },
                    new RowDefinition()
                }
            };

            boxView = new BoxView { Color = Colors.Black };
            rootGrid.Add(boxView);

            // Child page layout
            Grid childGrid = new Grid
            {
                Margin = new Thickness(20),
                RowDefinitions =
                {
                    new RowDefinition(),
                    new RowDefinition(),
                    new RowDefinition(),
                    new RowDefinition(),
                    new RowDefinition(),
                    new RowDefinition()
                }
            };

            DoubleToIntConverter doubleToInt = new DoubleToIntConverter();

            redSlider = new Slider();
            redSlider.ValueChanged += OnSliderValueChanged;
            childGrid.Add(redSlider);

            Label redLabel = new Label();
            redLabel.SetBinding(Label.TextProperty, new Binding("Value", converter: doubleToInt, converterParameter: "255", stringFormat: "Red = {0}", source: redSlider));
            Grid.SetRow(redLabel, 1);
            childGrid.Add(redLabel);

            greenSlider = new Slider();
            greenSlider.ValueChanged += OnSliderValueChanged;
            Grid.SetRow(greenSlider, 2);
            childGrid.Add(greenSlider);

            Label greenLabel = new Label();
            greenLabel.SetBinding(Label.TextProperty, new Binding("Value", converter: doubleToInt, converterParameter: "255", stringFormat: "Green = {0}", source: greenSlider));
            Grid.SetRow(greenLabel, 3);
            childGrid.Add(greenLabel);

            blueSlider = new Slider();
            blueSlider.ValueChanged += OnSliderValueChanged;
            Grid.SetRow(blueSlider, 4);
            childGrid.Add(blueSlider);

            Label blueLabel = new Label();
            blueLabel.SetBinding(Label.TextProperty, new Binding("Value", converter: doubleToInt, converterParameter: "255", stringFormat: "Blue = {0}", source: blueSlider));
            Grid.SetRow(blueLabel, 5);
            childGrid.Add(blueLabel);

            // Place the child Grid in the root Grid
            rootGrid.Add(childGrid, 0, 1);

            Title = "Nested Grids demo";
            Content = rootGrid;
        }

        void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            boxView.Color = new Color((float)redSlider.Value, (float)greenSlider.Value, (float)blueSlider.Value);
        }
    }
}
