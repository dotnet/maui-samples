namespace BindableLayoutDemos.Templates
{
    public class TechItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate MAUITemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return (string)item == ".NET MAUI" ? MAUITemplate : DefaultTemplate;
        }
    }
}
