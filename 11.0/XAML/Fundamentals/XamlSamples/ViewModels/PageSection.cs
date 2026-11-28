namespace XamlSamples
{
    // A group of demo pages. The first five follow the parts of the XAML fundamentals
    // tutorial, in order. The last one shows XAML features that are new in .NET 11.
    public class PageSection
    {
        public PageSection(string name, string docsUrl, Color accent, string note, params PageDataViewModel[] pages)
        {
            Name = name;
            DocsUrl = docsUrl;
            Accent = accent;
            Note = note;
            Pages = pages;

            foreach (var page in pages)
            {
                page.Section = this;
            }
        }

        public string Name { get; }

        public string DocsUrl { get; }

        public Color Accent { get; }

        public Brush AccentBrush => new SolidColorBrush(Accent);

        public string Note { get; }

        public bool HasNote => !string.IsNullOrEmpty(Note);

        public IReadOnlyList<PageDataViewModel> Pages { get; }

        const string TutorialUrl = "https://learn.microsoft.com/dotnet/maui/xaml/fundamentals/";

        public static IReadOnlyList<PageSection> All { get; } =
        [
            new PageSection("Get started", TutorialUrl + "get-started", Color.FromArgb("#7C5CFF"), null,
                new PageDataViewModel(typeof(HelloXamlPage), "Hello, XAML",
                    "Display a Label with many properties set",
                    "Each XML attribute sets a property of the Label: its text, alignment, rotation, font size, bold, and color.",
                    MaterialIcons.WavingHand),
                new PageDataViewModel(typeof(XamlPlusCodePage), "XAML + code",
                    "Interact with a Slider and Button",
                    "x:Name lets the code-behind file use the Label. The Slider's ValueChanged event and the Button's Clicked event run C# handlers. Drag the slider or tap the button.",
                    MaterialIcons.Code)),

            new PageSection("Essential syntax", TutorialUrl + "essential-syntax", Color.FromArgb("#3B82F6"), null,
                new PageDataViewModel(typeof(GridDemoPage), "Grid",
                    "Property elements and attached properties",
                    "Grid.RowDefinitions and Grid.ColumnDefinitions are property elements. Grid.Row, Grid.Column, Grid.RowSpan, and Grid.ColumnSpan are attached properties that place each child.",
                    MaterialIcons.GridView),
                new PageDataViewModel(typeof(AbsoluteDemoPage), "AbsoluteLayout",
                    "Position children with proportional bounds",
                    "AbsoluteLayout.LayoutBounds and AbsoluteLayout.LayoutFlags are attached properties. With proportional flags, each BoxView's size is a fraction of the layout, and its position is a fraction of the space left over: 0 is the start and 1 is the end.",
                    MaterialIcons.ViewQuilt)),

            new PageSection("Markup extensions", TutorialUrl + "markup-extensions", Color.FromArgb("#0D9488"), null,
                new PageDataViewModel(typeof(SharedResourcesPage), "Shared resources",
                    "Share values with StaticResource",
                    "The buttons get their values from ContentPage.Resources with StaticResource. OnPlatform picks a different text color on each platform.",
                    MaterialIcons.Style),
                new PageDataViewModel(typeof(StaticConstantsPage), "Static constants",
                    "Use the x:Static markup extension",
                    "x:Static reads static fields: the colors come from an AppConstants class, and the BoxView's size comes from Math.PI and Math.E, scaled up 100 times.",
                    MaterialIcons.Functions)),

            new PageSection("Data binding basics", TutorialUrl + "data-binding-basics", Color.FromArgb("#EA580C"), null,
                new PageDataViewModel(typeof(SliderBindingsPage), "Slider bindings",
                    "Bind properties of two views on the page",
                    "x:Reference makes the Slider the BindingContext of both Labels. Drag the slider to rotate the text.",
                    MaterialIcons.Tune),
                new PageDataViewModel(typeof(SliderTransformsPage), "Slider transforms",
                    "Use Sliders with reverse bindings",
                    "Each Slider binds to the Label with the OneWayToSource or TwoWay binding mode, so the Sliders set the Label's scale and rotations.",
                    MaterialIcons.Rotate3D),
                new PageDataViewModel(typeof(CollectionViewDemoPage), "CollectionView",
                    "Use a CollectionView with data bindings",
                    "x:Static sets ItemsSource to every named color. The DataTemplate binds to each NamedColor, and FloatToIntConverter shows its components as hexadecimal values.",
                    MaterialIcons.Palette)),

            new PageSection("Data binding and MVVM", TutorialUrl + "mvvm", Color.FromArgb("#0284C7"), null,
                new PageDataViewModel(typeof(OneShotDateTimePage), "One-shot DateTime",
                    "Obtain the current DateTime and display it",
                    "x:Static sets DateTime.Now as the BindingContext. The Labels inherit it, and read it once, when the page is created.",
                    MaterialIcons.Today),
                new PageDataViewModel(typeof(ClockPage), "Clock",
                    "Dynamically display the current time",
                    "ClockViewModel raises PropertyChanged every second, so the bound Label always shows the current time.",
                    MaterialIcons.Schedule),
                new PageDataViewModel(typeof(HslColorScrollPage), "HSL color scroll",
                    "Use a view model to select HSL colors",
                    "HslViewModel converts between a Color and its hue, saturation, and luminosity. The Sliders bind TwoWay, so dragging one updates the color and the other values.",
                    MaterialIcons.Colorize),
                new PageDataViewModel(typeof(KeypadPage), "Keypad",
                    "Use a view model for numeric keypad logic",
                    "Each Button binds to a command. AddCharCommand receives the CommandParameter, and DeleteCharCommand's CanExecute disables backspace when there's nothing to delete.",
                    MaterialIcons.Dialpad)),

            new PageSection("New in .NET 11", "https://learn.microsoft.com/dotnet/maui/whats-new/dotnet-11", Color.FromArgb("#EC4899"),
                "XAML source generation is now the default in Debug and Release builds, and resource dictionaries create each resource the first time it's used. You get both without changing your code.",
                new PageDataViewModel(typeof(ImplicitXmlnsPage), "Implicit namespaces",
                    "XAML without xmlns declarations",
                    "This page's XAML has no xmlns declarations. The .NET MAUI and x: namespaces are implicit, and an XmlnsDefinition attribute makes this app's types, such as HslViewModel, available without a prefix.",
                    MaterialIcons.DataObject,
                    "https://learn.microsoft.com/dotnet/maui/xaml/namespaces/"),
                new PageDataViewModel(typeof(CSharpExpressionsPage), "C# expressions",
                    "Compute values and handle events in XAML",
                    "Property values can be C# expressions. They're compiled bindings that update when the view model changes. Event attributes can be lambdas, which run in the page class, so they reach the view model through the page's Order property.",
                    MaterialIcons.AutoAwesome,
                    "https://learn.microsoft.com/dotnet/maui/xaml/csharp-expressions"),
                new PageDataViewModel(typeof(AncestorBindingsPage), "Ancestor bindings",
                    "RelativeSource bindings without reflection",
                    "Each + button is in an item template, but binds to a command on the page's view model with RelativeSource AncestorType. The source generator now compiles this binding, so it's safe to trim.",
                    MaterialIcons.AccountTree,
                    "https://learn.microsoft.com/dotnet/maui/fundamentals/data-binding/compiled-bindings"),
                new PageDataViewModel(typeof(ConverterCulturePage), "ConverterCulture",
                    "Pick the culture a converter uses",
                    "Every row binds to the same values with the same converter. Each binding's ConverterCulture sets the culture that the converter formats them in. Drag the slider.",
                    MaterialIcons.Language,
                    "https://learn.microsoft.com/dotnet/maui/fundamentals/data-binding/converters")),
        ];
    }
}
