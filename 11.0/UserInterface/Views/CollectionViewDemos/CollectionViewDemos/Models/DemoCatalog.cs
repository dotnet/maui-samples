using CollectionViewDemos.Views;

namespace CollectionViewDemos.Models;

// The demo pages shown on the home page, grouped by category.
public static class DemoCatalog
{
    public static IReadOnlyList<DemoCategory> Categories { get; }
    public static IReadOnlyList<DemoInfo> Demos { get; }

    static DemoCatalog()
    {
        var layout = new DemoCategory("Layout", AccentColor("Blue"));
        var spacing = new DemoCategory("Spacing", AccentColor("Teal"));
        var sizing = new DemoCategory("Sizing", AccentColor("Sky"));
        var selection = new DemoCategory("Selection", AccentColor("Violet"));
        var emptyView = new DemoCategory("Empty view", AccentColor("Orange"));
        var grouping = new DemoCategory("Grouping", AccentColor("Green"));
        var refresh = new DemoCategory("Pull to refresh", AccentColor("Pink"));
        var headers = new DemoCategory("Headers and footers", AccentColor("DeepViolet"));
        var scrolling = new DemoCategory("Scrolling", AccentColor("Rose"));
        var swipe = new DemoCategory("Swipe", AccentColor("DeepTeal"));

        Categories = [layout, spacing, sizing, selection, emptyView, grouping, refresh, headers, scrolling, swipe];

        Demos =
        [
            new("Vertical list (text)", "Strings without an ItemTemplate", MaterialIcons.ViewList, typeof(VerticalListTextPage), layout),
            new("Horizontal list (text)", "Strings in a horizontal list", MaterialIcons.ViewWeek, typeof(HorizontalListTextPage), layout),
            new("Vertical grid (text)", "Strings in a vertical grid", MaterialIcons.GridView, typeof(VerticalGridTextPage), layout),
            new("Horizontal grid (text)", "Strings in a horizontal grid", MaterialIcons.ViewModule, typeof(HorizontalGridTextPage), layout),
            new("Vertical list", "Items rendered with a DataTemplate", MaterialIcons.ViewAgenda, typeof(VerticalListPage), layout),
            new("Horizontal list", "A DataTemplate in a horizontal list", MaterialIcons.ViewArray, typeof(HorizontalListPage), layout),
            new("Vertical grid", "A DataTemplate in a two-column grid", MaterialIcons.Apps, typeof(VerticalGridPage), layout),
            new("Horizontal grid", "A DataTemplate in a four-row grid", MaterialIcons.GridOn, typeof(HorizontalGridPage), layout),
            new("DataTemplateSelector", "Choose a template for each item at runtime", MaterialIcons.AltRoute, typeof(VerticalListDataTemplateSelectorPage), layout),
            new("Right-to-left", "Mirror the layout with FlowDirection", MaterialIcons.FormatTextdirectionRToL, typeof(VerticalListRTLPage), layout),

            new("Vertical list spacing", "Space items with ItemSpacing", MaterialIcons.FormatLineSpacing, typeof(VerticalListSpacingPage), spacing),
            new("Horizontal list spacing", "ItemSpacing in a horizontal list", MaterialIcons.SpaceBar, typeof(HorizontalListSpacingPage), spacing),
            new("Vertical grid spacing", "HorizontalItemSpacing and VerticalItemSpacing", MaterialIcons.Grid4x4, typeof(VerticalGridSpacingPage), spacing),
            new("Horizontal grid spacing", "Grid item spacing in a horizontal grid", MaterialIcons.Padding, typeof(HorizontalGridSpacingPage), spacing),

            new("Variable size items", "Each item sizes to its own content", MaterialIcons.Height, typeof(VerticalListVariableSizeItemsPage), sizing),
            new("Dynamic item sizing", "Resize an item at runtime", MaterialIcons.OpenInFull, typeof(VerticalListDynamicSizeItemsPage), sizing),

            new("Single selection", "Select one item at a time", MaterialIcons.CheckCircle, typeof(VerticalListSingleSelectionPage), selection),
            new("Multiple selection", "Select several items", MaterialIcons.DoneAll, typeof(VerticalListMultipleSelectionPage), selection),
            new("Single pre-selection", "Start with an item selected", MaterialIcons.RadioButtonChecked, typeof(VerticalListSinglePreSelectionPage), selection),
            new("Multiple pre-selection", "Start with several items selected", MaterialIcons.LibraryAddCheck, typeof(VerticalListMultiplePreSelectionPage), selection),
            new("Selection color", "Style the Selected visual state", MaterialIcons.Palette, typeof(VerticalListSelectionColorPage), selection),

            new("Null ItemsSource", "Show a string when there is no data", MaterialIcons.Block, typeof(EmptyViewNullPage), emptyView),
            new("Load simulation", "Show a string while data loads", MaterialIcons.HourglassEmpty, typeof(EmptyViewLoadSimulationPage), emptyView),
            new("Filtering", "Show a string when a filter matches nothing", MaterialIcons.FilterAlt, typeof(EmptyViewFilteredPage), emptyView),
            new("Filtering with views", "Use a layout of views as the EmptyView", MaterialIcons.SearchOff, typeof(EmptyViewWithViewsFilteredPage), emptyView),
            new("EmptyViewTemplate", "Template the EmptyView with data", MaterialIcons.Inbox, typeof(EmptyViewTemplatePage), emptyView),
            new("EmptyView template selector", "Choose an EmptyViewTemplate at runtime", MaterialIcons.AltRoute, typeof(EmptyViewDataTemplateSelectorPage), emptyView),
            new("Swap EmptyViews", "Change the EmptyView at runtime", MaterialIcons.SwapHoriz, typeof(EmptyViewSwapPage), emptyView),

            new("Grouped list", "Group headers and footers from DataTemplates", MaterialIcons.Folder, typeof(VerticalListGroupingPage), grouping),
            new("Grouped list (text)", "Groups without templates", MaterialIcons.List, typeof(VerticalListTextGroupingPage), grouping),
            new("Empty groups", "Include groups that have no items", MaterialIcons.FolderOpen, typeof(VerticalListEmptyGroupsPage), grouping),
            new("Grouped variable size items", "Groups of items with different heights", MaterialIcons.UnfoldMore, typeof(VerticalListGroupingVariableSizeItemsPage), grouping),

            new("Refresh a vertical list", "Pull down to refresh with RefreshView", MaterialIcons.Refresh, typeof(VerticalListPullToRefreshPage), refresh),
            new("Refresh a horizontal grid", "RefreshView around a horizontal grid", MaterialIcons.Sync, typeof(HorizontalGridPullToRefreshPage), refresh),

            new("String header and footer", "Header and Footer set to strings", MaterialIcons.Title, typeof(VerticalListHeaderFooterStringPage), headers),
            new("Templated header and footer", "HeaderTemplate and FooterTemplate", MaterialIcons.WebAsset, typeof(VerticalListHeaderFooterDataTemplatePage), headers),
            new("View header and footer", "Header and Footer set to views", MaterialIcons.ViewDay, typeof(VerticalListHeaderFooterViewPage), headers),
            new("Horizontal grid header and footer", "Header and footer views in a horizontal grid", MaterialIcons.ViewSidebar, typeof(HorizontalGridHeaderFooterViewPage), headers),

            new("Scroll to an index", "ScrollTo an item by its index", MaterialIcons.FormatListNumbered, typeof(ScrollToByIndexPage), scrolling),
            new("Scroll to an item", "ScrollTo an item object", MaterialIcons.MyLocation, typeof(ScrollToByObjectPage), scrolling),
            new("Scroll to an index in a group", "ScrollTo by index in grouped data", MaterialIcons.FormatListNumbered, typeof(ScrollToByIndexWithGroupingPage), scrolling),
            new("Scroll to an item in a group", "ScrollTo an item in grouped data", MaterialIcons.ManageSearch, typeof(ScrollToByObjectWithGroupingPage), scrolling),
            new("Snap points", "Snap items into place after scrolling", MaterialIcons.VerticalAlignCenter, typeof(VerticalListSnapPointsPage), scrolling),
            new("ItemsUpdatingScrollMode", "Control scrolling when items are added", MaterialIcons.PlaylistAdd, typeof(ItemsUpdatingScrollModePage), scrolling),
            new("Incremental loading", "Load more data as you scroll", MaterialIcons.Downloading, typeof(IncrementalLoadingPage), scrolling),

            new("Context menu items", "Swipe or right-click for item actions", MaterialIcons.Swipe, typeof(VerticalListSwipeContextItemsPage), swipe),
        ];
    }

    static Color AccentColor(string key) =>
        Application.Current?.Resources.TryGetValue(key, out var value) == true && value is Color color
            ? color
            : Colors.Gray;
}

public class DemoCategory
{
    public DemoCategory(string name, Color accent)
    {
        Name = name;
        Accent = accent;
        AccentBrush = new SolidColorBrush(accent);
    }

    public string Name { get; }
    public Color Accent { get; }
    public Brush AccentBrush { get; }
}

public class DemoInfo
{
    public DemoInfo(string title, string description, string glyph, Type pageType, DemoCategory category)
    {
        Title = title;
        Description = description;
        Glyph = glyph;
        PageType = pageType;
        Category = category;
    }

    public string Title { get; }
    public string Description { get; }
    public string Glyph { get; }
    public Type PageType { get; }
    public DemoCategory Category { get; }
}

public class DemoGroup : List<DemoInfo>
{
    public DemoGroup(DemoCategory category, IEnumerable<DemoInfo> demos) : base(demos)
    {
        Category = category;
    }

    public DemoCategory Category { get; }
    public string Name => Category.Name;
}
