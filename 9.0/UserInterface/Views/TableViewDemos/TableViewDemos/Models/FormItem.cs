namespace TableViewDemos.Models
{
    public class FormSection
    {
        public string Title { get; set; }
        public List<FormField> Fields { get; set; } = new();
    }

    public class FormField
    {
        public string Label { get; set; }
        public string Placeholder { get; set; }
        public string Value { get; set; }
        public bool IsPassword { get; set; }
    }
} 