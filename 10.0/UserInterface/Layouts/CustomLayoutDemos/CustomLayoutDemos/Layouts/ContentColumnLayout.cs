using Microsoft.Maui.Layouts;

namespace CustomLayoutDemos.Layouts
{
    public class ContentColumnLayout : Layout, IGridLayout
    {
        public IReadOnlyList<IGridRowDefinition> RowDefinitions { get; }
        public IReadOnlyList<IGridColumnDefinition> ColumnDefinitions { get; }

        public double RowSpacing => 0;
        public double ColumnSpacing => 0;

        public ContentColumnLayout(IView header, IView content, IView footer)
        {
            var gridColumnDefinitions = new List<IGridColumnDefinition>(1)
            {
                new ColumnDefinition(GridLength.Star)
            };

            var gridRowDefinitions = new List<IGridRowDefinition>(3)
            {
                new RowDefinition(GridLength.Auto), // Header
                new RowDefinition(GridLength.Star), // Content
                new RowDefinition(GridLength.Auto)  // Footer
            };

            ColumnDefinitions = gridColumnDefinitions.AsReadOnly();
            RowDefinitions = gridRowDefinitions.AsReadOnly();

            Add(header);
            Add(content);
            Add(footer);
        }

        public int GetColumn(IView view) => 0;

        public int GetColumnSpan(IView view) => 1;

        public int GetRow(IView view) => IndexOf(view);

        public int GetRowSpan(IView view) => 1;

        protected override ILayoutManager CreateLayoutManager()
        {
            return new GridLayoutManager(this);
        }
    }
}
