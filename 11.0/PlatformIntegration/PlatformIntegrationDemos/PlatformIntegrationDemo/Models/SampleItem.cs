namespace PlatformIntegrationDemo.Models;

public class SampleCategory
{
    public SampleCategory(string name, Color accent)
    {
        Name = name;
        Accent = accent;
        AccentBrush = new SolidColorBrush(accent);
    }

    public string Name { get; }
    public Color Accent { get; }
    public Brush AccentBrush { get; }
}

public class SampleItem
{
    public SampleItem(string glyph, string name, Type pageType, string description, SampleCategory category, params string[] tags)
    {
        Glyph = glyph;
        Name = name;
        PageType = pageType;
        Description = description;
        Category = category;
        Tags = tags ?? [];
    }

    public string Glyph { get; }
    public string Name { get; }
    public string Description { get; }
    public Type PageType { get; }
    public SampleCategory Category { get; }
    public string[] Tags { get; }
}

public class SampleGroup : List<SampleItem>
{
    public SampleGroup(SampleCategory category, IEnumerable<SampleItem> samples) : base(samples)
    {
        Category = category;
    }

    public SampleCategory Category { get; }
    public string Name => Category.Name;
}
