namespace XamlSamples
{
    // One demo page in the catalog. MainPage lists these, and DemoPageHeader shows
    // the title and lead of the page that's open.
    public class PageDataViewModel
    {
        public PageDataViewModel(Type type, string title, string description, string lead, string glyph, string docsUrl = null)
        {
            Type = type;
            Title = title;
            Description = description;
            Lead = lead;
            Glyph = glyph;
            this.docsUrl = docsUrl;
        }

        readonly string docsUrl;

        public Type Type { private set; get; }

        public string Title { private set; get; }

        // Short summary shown on the home page.
        public string Description { private set; get; }

        // Longer explanation shown above the demo.
        public string Lead { private set; get; }

        public string Glyph { private set; get; }

        public PageSection Section { internal set; get; }

        public string DocsUrl => docsUrl ?? Section.DocsUrl;

        public string DocsLinkText => docsUrl is null ? "Read the tutorial" : "Learn more";

        public static IList<PageDataViewModel> All => PageSection.All.SelectMany(section => section.Pages).ToList();

        public static PageDataViewModel Find(Type pageType) => All.FirstOrDefault(page => page.Type == pageType);
    }
}
