namespace BindableLayoutDemos.Templates
{
    public class AchievementsDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate OtherTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            bool result = (bool)item;
            return result ? OtherTemplate : DefaultTemplate;            
        }
    }
}
