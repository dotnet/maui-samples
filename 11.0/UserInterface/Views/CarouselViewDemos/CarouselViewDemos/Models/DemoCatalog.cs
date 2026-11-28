using CarouselViewDemos.Views;

namespace CarouselViewDemos.Models;

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
        var interaction = new DemoCategory("Interaction", AccentColor("Violet"));
        var indicators = new DemoCategory("Indicators", AccentColor("Green"));
        var emptyView = new DemoCategory("Empty view", AccentColor("Orange"));
        var scrolling = new DemoCategory("Scrolling", AccentColor("Rose"));
        var refresh = new DemoCategory("Pull to refresh", AccentColor("Pink"));
        var swipe = new DemoCategory("Swipe", AccentColor("DeepTeal"));

        Categories = [layout, spacing, sizing, interaction, indicators, emptyView, scrolling, refresh, swipe];

        Demos =
        [
            new("Horizontal (text)", "Strings without an ItemTemplate", MaterialIcons.TextFields, typeof(HorizontalTextLayoutPage), layout),
            new("Vertical (text)", "Strings in a vertical carousel", MaterialIcons.SwapVert, typeof(VerticalTextLayoutPage), layout),
            new("Horizontal layout", "Items rendered with a DataTemplate", MaterialIcons.ViewCarousel, typeof(HorizontalTemplateLayoutPage), layout),
            new("Vertical layout", "A DataTemplate in a vertical carousel", MaterialIcons.ViewDay, typeof(VerticalTemplateLayoutPage), layout),
            new("Peek items", "Show adjacent items with PeekAreaInsets", MaterialIcons.ViewColumn, typeof(HorizontalLayoutPeekAreaPage), layout),
            new("DataTemplateSelector", "Choose a template for each item at runtime", MaterialIcons.AltRoute, typeof(HorizontalLayoutDataTemplateSelectorPage), layout),
            new("Right-to-left", "Mirror the layout with FlowDirection", MaterialIcons.FormatTextdirectionRToL, typeof(HorizontalTemplateLayoutRTLPage), layout),

            new("Horizontal spacing", "Space items with ItemSpacing", MaterialIcons.SpaceBar, typeof(HorizontalSpacingPage), spacing),
            new("Vertical spacing", "ItemSpacing in a vertical carousel", MaterialIcons.FormatLineSpacing, typeof(VerticalSpacingPage), spacing),

            new("Dynamic item sizing", "Resize an item at runtime", MaterialIcons.OpenInFull, typeof(DynamicSizeItemsPage), sizing),

            new("Current item and position", "React to CurrentItemChanged and PositionChanged", MaterialIcons.TouchApp, typeof(CurrentItemAndPositionPage), interaction),
            new("Current item and position bindings", "Bind the changed commands to a view model", MaterialIcons.Link, typeof(CurrentItemAndPositionBindingsPage), interaction),
            new("Preset item and position", "Set CurrentItem and Position from a view model", MaterialIcons.MyLocation, typeof(SetCurrentItemAndPositionPage), interaction),
            new("Visual states", "Style the current, previous, and next items", MaterialIcons.AutoAwesomeMotion, typeof(VisualStatesPage), interaction),
            new("Disable loop", "Stop at the first and last item", MaterialIcons.Loop, typeof(LoopPage), interaction),

            new("IndicatorView", "Show the position with indicators", MaterialIcons.MoreHoriz, typeof(HorizontalLayoutWithIndicatorsPage), indicators),

            new("Null ItemsSource", "Show a string when there is no data", MaterialIcons.Block, typeof(EmptyViewNullPage), emptyView),
            new("Load simulation", "Show a string while data loads", MaterialIcons.HourglassEmpty, typeof(EmptyViewLoadSimulationPage), emptyView),
            new("Filtering", "Show a string when a filter matches nothing", MaterialIcons.FilterAlt, typeof(EmptyViewFilteredPage), emptyView),
            new("Filtering with views", "Use a layout of views as the EmptyView", MaterialIcons.SearchOff, typeof(EmptyViewWithViewsFilteredPage), emptyView),
            new("EmptyViewTemplate", "Template the EmptyView with data", MaterialIcons.Inbox, typeof(EmptyViewTemplatePage), emptyView),
            new("EmptyView template selector", "Choose an EmptyViewTemplate at runtime", MaterialIcons.AltRoute, typeof(EmptyViewDataTemplateSelectorPage), emptyView),
            new("Swap EmptyViews", "Change the EmptyView at runtime", MaterialIcons.SwapHoriz, typeof(EmptyViewSwapPage), emptyView),

            new("Scroll to an index", "ScrollTo an item by its index", MaterialIcons.FormatListNumbered, typeof(ScrollToByIndexPage), scrolling),
            new("Scroll to an item", "ScrollTo an item object", MaterialIcons.ManageSearch, typeof(ScrollToByObjectPage), scrolling),
            new("Snap points", "SnapPointsType and SnapPointsAlignment", MaterialIcons.CenterFocusStrong, typeof(SnapPointsPage), scrolling),
            new("ItemsUpdatingScrollMode", "Control scrolling when items are added", MaterialIcons.PlaylistAdd, typeof(ItemsUpdatingScrollModePage), scrolling),
            new("Incremental loading", "Load more data as you scroll", MaterialIcons.Downloading, typeof(IncrementalLoadingPage), scrolling),

            new("Refresh a horizontal carousel", "Pull down to refresh with RefreshView", MaterialIcons.Refresh, typeof(HorizontalPullToRefreshPage), refresh),
            new("Refresh a vertical carousel", "RefreshView around a vertical carousel", MaterialIcons.Sync, typeof(VerticalPullToRefreshPage), refresh),

            new("Swipe items", "Swipe up or down on an item for actions", MaterialIcons.SwipeVertical, typeof(HorizontalSwipeItemsPage), swipe),
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
