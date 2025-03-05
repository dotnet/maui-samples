namespace TableViewDemos.Models
{
    public class SettingsSection
    {
        public string Title { get; set; }
        public List<SettingsItem> Items { get; set; } = new();
    }

    public class SettingsItem
    {
        public string Label { get; set; }
        public string Placeholder { get; set; }
        public bool IsSwitch { get; set; }
        public bool IsEntry { get; set; }
        public bool IsSlider { get; set; }
        public bool SwitchValue { get; set; }
        public string EntryValue { get; set; }
        public double SliderValue { get; set; }
        public double SliderMinimum { get; set; }
        public double SliderMaximum { get; set; }
    }
} 