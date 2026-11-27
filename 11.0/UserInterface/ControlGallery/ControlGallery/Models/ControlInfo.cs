namespace ControlGallery.Models;

public class ControlCategory
{
    public ControlCategory(string name, Color accent)
    {
        Name = name;
        Accent = accent;
        AccentBrush = new SolidColorBrush(accent);
    }

    public string Name { get; }
    public Color Accent { get; }
    public Brush AccentBrush { get; }
}

public class ControlInfo
{
    public ControlInfo(string name, string description, string glyph, Type pageType, ControlCategory category)
    {
        Name = name;
        Description = description;
        Glyph = glyph;
        PageType = pageType;
        Category = category;
    }

    public string Name { get; }
    public string Description { get; }
    public string Glyph { get; }
    public Type PageType { get; }
    public ControlCategory Category { get; }
}

public class ControlGroup : List<ControlInfo>
{
    public ControlGroup(ControlCategory category, IEnumerable<ControlInfo> controls) : base(controls)
    {
        Category = category;
    }

    public ControlCategory Category { get; }
    public string Name => Category.Name;
}
