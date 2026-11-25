using ControlGallery.Views.XAML;

namespace ControlGallery.Models;

// The list of demo pages shown on the home page, grouped by category.
public static class ControlCatalog
{
    public static IReadOnlyList<ControlCategory> Categories { get; }
    public static IReadOnlyList<ControlInfo> Controls { get; }

    static ControlCatalog()
    {
        var layouts = new ControlCategory("Layouts", AccentColor("Blue"));
        var input = new ControlCategory("Input", AccentColor("Violet"));
        var display = new ControlCategory("Display", AccentColor("Teal"));
        var collections = new ControlCategory("Collections", AccentColor("Orange"));
        var shapes = new ControlCategory("Shapes & graphics", AccentColor("Pink"));
        var pages = new ControlCategory("Pages", AccentColor("Sky"));
        var web = new ControlCategory("Web", AccentColor("Green"));

        Categories = [layouts, input, display, collections, shapes, pages, web];

        Controls =
        [
            new("AbsoluteLayout", "Arrange children by coordinate positions", MaterialIcons.OpenWith, typeof(AbsoluteLayoutDemoPage), layouts),
            new("Border", "Draw a border or background around a control", MaterialIcons.BorderStyle, typeof(BorderDemoPage), layouts),
            new("ContentView", "Host a single child", MaterialIcons.CropFree, typeof(ContentViewDemoPage), layouts),
            new("FlexLayout", "Arrange children in a stack or a wrapped stack", MaterialIcons.Dashboard, typeof(FlexLayoutDemoPage), layouts),
            new("Grid", "Arrange children in rows and columns", MaterialIcons.GridView, typeof(GridDemoPage), layouts),
            new("HorizontalStackLayout", "Arrange children in a horizontal stack", MaterialIcons.ViewColumn, typeof(HorizontalStackLayoutDemoPage), layouts),
            new("RelativeLayout", "Arrange children relative to each other", MaterialIcons.DeviceHub, typeof(RelativeLayoutDemoPage), layouts),
            new("ScrollView", "Scroll content that is too large for the page", MaterialIcons.SwapVert, typeof(ScrollViewDemoPage), layouts),
            new("StackLayout", "Arrange children in a stack", MaterialIcons.ViewAgenda, typeof(StackLayoutDemoPage), layouts),
            new("VerticalStackLayout", "Arrange children in a vertical stack", MaterialIcons.ViewStream, typeof(VerticalStackLayoutDemoPage), layouts),

            new("Button", "Initiate a command", MaterialIcons.SmartButton, typeof(ButtonDemoPage), input),
            new("CheckBox", "Select a Boolean value", MaterialIcons.CheckBox, typeof(CheckBoxPage), input),
            new("DatePicker", "Select a date", MaterialIcons.Event, typeof(DatePickerDemoPage), input),
            new("Editor", "Edit multiple lines of text", MaterialIcons.Notes, typeof(EditorDemoPage), input),
            new("Entry", "Edit a single line of text", MaterialIcons.Edit, typeof(EntryDemoPage), input),
            new("ImageButton", "Initiate a command with an image", MaterialIcons.TouchApp, typeof(ImageButtonDemoPage), input),
            new("Picker", "Select an item from a list", MaterialIcons.ArrowDropDownCircle, typeof(PickerDemoPage), input),
            new("RadioButton", "Select one option from a set", MaterialIcons.RadioButtonChecked, typeof(RadioButtonDemoPage), input),
            new("SearchBar", "Initiate a search", MaterialIcons.Search, typeof(SearchBarDemoPage), input),
            new("Slider", "Select a number from a continuous range", MaterialIcons.Tune, typeof(SliderDemoPage), input),
            new("Stepper", "Select a number from discrete increments", MaterialIcons.Exposure, typeof(StepperDemoPage), input),
            new("Switch", "Toggle a Boolean value", MaterialIcons.ToggleOn, typeof(SwitchDemoPage), input),
            new("TimePicker", "Select a time", MaterialIcons.Schedule, typeof(TimePickerDemoPage), input),

            new("ActivityIndicator", "Show that the app is busy", MaterialIcons.Autorenew, typeof(ActivityIndicatorDemoPage), display),
            new("BoxView", "Display a colored rectangle", MaterialIcons.CropSquare, typeof(BoxViewDemoPage), display),
            new("Image", "Display a bitmap", MaterialIcons.Image, typeof(ImageDemoPage), display),
            new("Label", "Display a text string", MaterialIcons.TextFields, typeof(LabelDemoPage), display),
            new("ProgressBar", "Show the progress of a task", MaterialIcons.LinearScale, typeof(ProgressBarDemoPage), display),

            new("CarouselView", "Present data in a scrollable carousel", MaterialIcons.ViewCarousel, typeof(CarouselViewDemoPage), collections),
            new("CollectionView", "Present a list or grid of data items", MaterialIcons.ViewList, typeof(CollectionViewDemoPage), collections),
            new("IndicatorView", "Show the position in a CarouselView", MaterialIcons.MoreHoriz, typeof(IndicatorViewDemoPage), collections),
            new("RefreshView", "Add pull-to-refresh to scrollable content", MaterialIcons.Refresh, typeof(RefreshViewDemoPage), collections),
            new("SwipeView", "Reveal actions by swiping an item", MaterialIcons.Swipe, typeof(SwipeViewDemoPage), collections),

            new("Ellipse", "Display an ellipse or circle", MaterialIcons.Circle, typeof(EllipseDemoPage), shapes),
            new("GraphicsView", "Draw 2D graphics on a canvas", MaterialIcons.Brush, typeof(GraphicsViewDemoPage), shapes),
            new("Line", "Display a line", MaterialIcons.HorizontalRule, typeof(LineDemoPage), shapes),
            new("Path", "Display curves and complex shapes", MaterialIcons.Gesture, typeof(PathDemoPage), shapes),
            new("Polygon", "Display a polygon", MaterialIcons.Pentagon, typeof(PolygonDemoPage), shapes),
            new("Polyline", "Display a series of connected lines", MaterialIcons.ShowChart, typeof(PolylineDemoPage), shapes),
            new("Rectangle", "Display a rectangle or square", MaterialIcons.Rectangle, typeof(RectangleDemoPage), shapes),

            new("ContentPage", "Present a single view", MaterialIcons.Article, typeof(ContentPageDemoPage), pages),
            new("FlyoutPage", "Present a list and a detail page", MaterialIcons.MenuOpen, typeof(FlyoutPageDemoPage), pages),
            new("NavigationPage", "Navigate through a stack of pages", MaterialIcons.AltRoute, typeof(NavigationPageDemoPage), pages),
            new("TabbedPage", "Present pages with tabs", MaterialIcons.Tab, typeof(TabbedPageDemoPage), pages),

            new("BlazorWebView", "Host a Blazor web app in your app", MaterialIcons.Code, typeof(BlazorWebViewDemoPage), web),
            new("WebView", "Display a web page", MaterialIcons.Public, typeof(WebViewDemoPage), web),
        ];
    }

    static Color AccentColor(string key) =>
        Application.Current?.Resources.TryGetValue(key, out var value) == true && value is Color color
            ? color
            : Colors.Gray;
}
