namespace DataBindingDemos.Models;

// The demo pages shown on the home page, grouped by category.
// The categories follow the order of the data binding docs.
public static class DemoCatalog
{
    public static IReadOnlyList<DemoCategory> Categories { get; }
    public static IReadOnlyList<DemoInfo> Demos { get; }

    static DemoCatalog()
    {
        var basics = new DemoCategory("Basic bindings", AccentColor("Blue"));
        var mode = new DemoCategory("Binding mode", AccentColor("Violet"));
        var formatting = new DemoCategory("String formatting", AccentColor("Teal"));
        var path = new DemoCategory("Binding path", AccentColor("Sky"));
        var converters = new DemoCategory("Value converters", AccentColor("Orange"));
        var relative = new DemoCategory("Relative bindings", AccentColor("Green"));
        var fallbacks = new DemoCategory("Binding fallbacks", AccentColor("Rose"));
        var multi = new DemoCategory("Multi-bindings", AccentColor("Pink"));
        var commanding = new DemoCategory("Commanding", AccentColor("DeepTeal"));
        var compiled = new DemoCategory("Compiled bindings", AccentColor("DeepViolet"));

        Categories = [basics, mode, formatting, path, converters, relative, fallbacks, multi, commanding, compiled];

        Demos =
        [
            new("Code binding", "SetBinding with a BindingContext", MaterialIcons.Code, typeof(BasicCodeBindingPage), basics),
            new("XAML binding", "{Binding} with a BindingContext", MaterialIcons.DataObject, typeof(BasicXamlBindingPage), basics),
            new("Code binding with a source", "SetBinding with a source object", MaterialIcons.Link, typeof(AlternativeCodeBindingPage), basics),
            new("XAML binding with a source", "Binding with a Source object", MaterialIcons.DataArray, typeof(AlternativeXamlBindingPage), basics),
            new("BindingContext inheritance", "Children inherit the BindingContext", MaterialIcons.AccountTree, typeof(BindingContextInheritancePage), basics),

            new("Reverse binding", "Bind the source to the target", MaterialIcons.SwapHoriz, typeof(ReverseBindingPage), mode),
            new("Simple color selector", "TwoWay bindings to a view model", MaterialIcons.Palette, typeof(SimpleColorSelectorPage), mode),

            new("String formatting", "StringFormat with .NET format strings", MaterialIcons.TextFormat, typeof(StringFormattingPage), formatting),
            new("Better color selector", "Format slider values with StringFormat", MaterialIcons.Tune, typeof(BetterColorSelectorPage), formatting),

            new("Path variations", "Sub-properties, indexers, and collections", MaterialIcons.Route, typeof(PathVariationsPage), path),

            new("Enable buttons", "Convert an int to a bool", MaterialIcons.TouchApp, typeof(EnableButtonsPage), converters),
            new("Switch indicators", "Convert a bool to text, a color, or a style", MaterialIcons.ToggleOn, typeof(SwitchIndicatorsPage), converters),
            new("RGB color selector", "Use a ConverterParameter", MaterialIcons.Colorize, typeof(RgbColorSelectorPage), converters),

            new("Self", "Bind to a property of the same element", MaterialIcons.Person, typeof(RelativeSourceSelfPage), relative),
            new("FindAncestor", "Bind to the view model of a parent", MaterialIcons.NorthWest, typeof(RelativeSourceFindAncestorPage), relative),
            new("TemplatedParent", "Bind to the control a template is applied to", MaterialIcons.DashboardCustomize, typeof(RelativeSourceTemplatedParentPage), relative),

            new("FallbackValue and TargetNullValue", "Replace missing and null values", MaterialIcons.HelpOutline, typeof(MonkeysPage), fallbacks),

            new("String formatting", "Combine values with StringFormat", MaterialIcons.FormatQuote, typeof(MultiBindingStringFormatPage), multi),
            new("String formatting in code", "Create a MultiBinding in C#", MaterialIcons.Code, typeof(MultiBindingStringFormatCodePage), multi),
            new("Multi-value converter", "Combine values with an IMultiValueConverter", MaterialIcons.CallMerge, typeof(MultiBindingConverterPage), multi),
            new("Multi-value converter in code", "Create the converter binding in C#", MaterialIcons.Merge, typeof(MultiBindingConverterCodePage), multi),
            new("Nested multi-bindings", "A MultiBinding inside a MultiBinding", MaterialIcons.Layers, typeof(NestedMultiBindingPage), multi),
            new("Fallback values", "FallbackValue and TargetNullValue", MaterialIcons.HelpOutline, typeof(MultiBindingFallbackValuePage), multi),
            new("RelativeSource multi-binding", "Bind to a templated parent", MaterialIcons.ExpandMore, typeof(RelativeSourceMultiBindingPage), multi),

            new("Person entry", "Commands that enable and disable buttons", MaterialIcons.PersonAdd, typeof(PersonEntryPage), commanding),
            new("Decimal keypad", "Commands with a CommandParameter", MaterialIcons.Dialpad, typeof(DecimalKeypadPage), commanding),

            new("Compiled color selector", "x:DataType on the page", MaterialIcons.Bolt, typeof(CompiledColorSelectorPage), compiled),
            new("Compiled color list", "x:DataType in a DataTemplate", MaterialIcons.ListAlt, typeof(CompiledColorListPage), compiled),
            new("Compiled and classic bindings", "Mix compiled and classic bindings", MaterialIcons.CompareArrows, typeof(MixedBindingsPage), compiled),
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
