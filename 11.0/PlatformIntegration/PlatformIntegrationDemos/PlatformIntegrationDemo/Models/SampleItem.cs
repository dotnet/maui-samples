namespace PlatformIntegrationDemo.Models
{
    public class SampleItem
    {
        public string Icon { get; }
        public string Name { get; }
        public string Description { get; }
        public Type PageType { get; }
        public string[] Tags { get; }

        public SampleItem(string icon, string name, Type pageType, string description, params string[] tags)
        {
            Icon = icon;
            Name = name;
            Description = description;
            PageType = pageType;
            Tags = tags ?? new string[0];
        }
    }
}

