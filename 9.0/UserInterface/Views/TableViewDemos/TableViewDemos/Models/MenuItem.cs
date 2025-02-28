namespace TableViewDemos.Models
{
    public class MenuSection
    {
        public string Title { get; set; }
        public List<MenuListItem> Items { get; set; } = new();
    }

    public class MenuListItem
    {
        public string Text { get; set; }
        public string Detail { get; set; }
        public string ImageSource { get; set; }
        public System.Windows.Input.ICommand Command { get; set; }
    }
} 