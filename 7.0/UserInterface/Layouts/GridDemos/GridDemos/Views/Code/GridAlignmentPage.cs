namespace GridDemos.Views.Code
{
    public class GridAlignmentPage : ContentPage
    {
        public GridAlignmentPage()
        {
            Grid grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition(),
                    new RowDefinition(),
                    new RowDefinition()
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition(),
                    new ColumnDefinition(),
                    new ColumnDefinition()
                }
            };

            // Row 0
            grid.Add(new BoxView
            {
                Color = Colors.AliceBlue
            });
            grid.Add(new Label
            {
                Text = "Upper left",
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start
            });

            grid.Add(new BoxView
            {
                Color = Colors.LightSkyBlue
            }, 1, 0);
            grid.Add(new Label
            {
                Text = "Upper center",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start
            }, 1, 0);

            grid.Add(new BoxView
            {
                Color = Colors.CadetBlue
            }, 2, 0);
            grid.Add(new Label
            {
                Text = "Upper right",
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start
            }, 2, 0);

            // Row 1
            grid.Add(new BoxView
            {
                Color = Colors.CornflowerBlue
            }, 0, 1);
            grid.Add(new Label
            {
                Text = "Center left",
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            }, 0, 1);

            grid.Add(new BoxView
            {
                Color = Colors.DodgerBlue
            }, 1, 1);
            grid.Add(new Label
            {
                Text = "Center center",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }, 1, 1);

            grid.Add(new BoxView
            {
                Color = Colors.DarkSlateBlue
            }, 2, 1);
            grid.Add(new Label
            {
                Text = "Center right",
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            }, 2, 1);

            // Row 2
            grid.Add(new BoxView
            {
                Color = Colors.SteelBlue
            }, 0, 2);
            grid.Add(new Label
            {
                Text = "Lower left",
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End
            }, 0, 2);

            grid.Add(new BoxView
            {
                Color = Colors.LightBlue
            }, 1, 2);
            grid.Add(new Label
            {
                Text = "Lower center",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End
            }, 1, 2);

            grid.Add(new BoxView
            {
                Color = Colors.BlueViolet
            }, 2, 2);
            grid.Add(new Label
            {
                Text = "Lower right",
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.End
            }, 2, 2);

            Title = "Grid alignment demo";
            Content = grid;
        }
    }
}
