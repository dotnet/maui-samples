﻿namespace ScaleAndRotate;

class ScaleDemoPage : ContentPage
{
    public ScaleDemoPage()
    {
        this.Title = "Scale";

        // Label to be transformed.
        Label label = new Label
        {
            Text = "SCALE",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        // Label and Slider for Scale property.
        Label scaleSliderValue = new Label
        {
            VerticalTextAlignment = TextAlignment.Center
        };
        Grid.SetRow(scaleSliderValue, 0);
        Grid.SetColumn(scaleSliderValue, 0);

        Slider scaleSlider = new Slider
        {
            Maximum = 10
        };
        Grid.SetRow(scaleSlider, 0);
        Grid.SetColumn(scaleSlider, 1);

        // Set Bindings.
        scaleSliderValue.BindingContext = scaleSlider;
        scaleSliderValue.SetBinding(Label.TextProperty,
            new Binding("Value", BindingMode.OneWay,
                null, null, "Scale = {0:F1}"));

        scaleSlider.BindingContext = label;
        scaleSlider.SetBinding(Slider.ValueProperty,
            new Binding("Scale", BindingMode.TwoWay));

        // Label and Slider for AnchorX property.
        Label anchorxStepperValue = new Label
        {
            VerticalTextAlignment = TextAlignment.Center
        };
        Grid.SetRow(anchorxStepperValue, 1);
        Grid.SetColumn(anchorxStepperValue, 0);

        Stepper anchorxStepper = new Stepper
        {
            Maximum = 2,
            Minimum = -1,
            Increment = 0.5
        };
        Grid.SetRow(anchorxStepper, 1);
        Grid.SetColumn(anchorxStepper, 1);

        // Set bindings.
        anchorxStepperValue.BindingContext = anchorxStepper;
        anchorxStepperValue.SetBinding(Label.TextProperty,
            new Binding("Value", BindingMode.OneWay,
                null, null, "AnchorX = {0:F1}"));

        anchorxStepper.BindingContext = label;
        anchorxStepper.SetBinding(Stepper.ValueProperty,
            new Binding("AnchorX", BindingMode.TwoWay));

        // Label and Slider for AnchorY property.
        Label anchoryStepperValue = new Label
        {
            VerticalTextAlignment = TextAlignment.Center
        };
        Grid.SetRow(anchoryStepperValue, 2);
        Grid.SetColumn(anchoryStepperValue, 0);

        Stepper anchoryStepper = new Stepper
        {
            Maximum = 2,
            Minimum = -1,
            Increment = 0.5
        };
        Grid.SetRow(anchoryStepper, 2);
        Grid.SetColumn(anchoryStepper, 1);

        // Set bindings.
        anchoryStepperValue.BindingContext = anchoryStepper;
        anchoryStepperValue.SetBinding(Label.TextProperty,
            new Binding("Value", BindingMode.OneWay,
                null, null, "AnchorY = {0:F1}"));

        anchoryStepper.BindingContext = label;
        anchoryStepper.SetBinding(Stepper.ValueProperty,
            new Binding("AnchorY", BindingMode.TwoWay));

        // Create controls grid
        Grid controlsGrid = new Grid
        {
            Padding = 10,
            RowDefinitions = {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnDefinitions = {
                new ColumnDefinition { Width = new GridLength (0.5, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength (0.5, GridUnitType.Star) }
            },
            Children = {
                scaleSliderValue,
                scaleSlider,
                anchorxStepperValue,
                anchorxStepper,
                anchoryStepperValue,
                anchoryStepper
            }
        };

        // Assemble the page with proper layout.
        this.Content = new Grid
        {
            RowDefinitions = {
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Auto }
            },
            Children = {
                label,
                controlsGrid
            }
        };

        // Set grid positions
        Grid.SetRow(label, 0);
        Grid.SetRow(controlsGrid, 1);
    }
}
