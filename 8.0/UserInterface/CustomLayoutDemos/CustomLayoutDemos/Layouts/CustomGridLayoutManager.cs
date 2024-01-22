using Microsoft.Maui.Layouts;

namespace CustomLayoutDemos.Layouts
{
    public class CustomGridLayoutManager : GridLayoutManager
    {
        public CustomGridLayoutManager(IGridLayout layout) : base(layout)
        {
        }

        public override Size Measure(double widthConstraint, double heightConstraint)
        {
            EnsureRows();
            return base.Measure(widthConstraint, heightConstraint);
        }

        void EnsureRows()
        {
            if (Grid is not Grid grid)
            {
                return;
            }

            // Find the maximum row value from the child views
            int maxRow = 0;
            foreach (var child in grid)
            {
                maxRow = Math.Max(grid.GetRow(child), maxRow);
            }

            // Add more rows if needed
            for (int n = grid.RowDefinitions.Count; n <= maxRow; n++)
            {
                grid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            }
        }
    }
}

