namespace BehaviorsDemos
{
    public class EventToCommandBehaviorPageCode : ContentPage
    {
        public EventToCommandBehaviorPageCode()
        {
            BindingContext = new HomePageViewModel();

            var listView = new CollectionView { SelectionMode = SelectionMode.Single };
            listView.SetBinding(ItemsView.ItemsSourceProperty, static (HomePageViewModel vm) => vm.People);
            listView.ItemTemplate = new DataTemplate(() =>
            {
                var label = new Label { Padding = 10 };
                label.SetBinding(Label.TextProperty, static (Person person) => person.Name);
                return label;
            });
            listView.Behaviors.Add(new EventToCommandBehavior
            {
                EventName = "SelectionChanged",
                Command = ((HomePageViewModel)BindingContext).OutputAgeCommand,
                Converter = new SelectedItemEventArgsToSelectedItemConverter()
            });

            var selectedItemLabel = new Label();
            selectedItemLabel.SetBinding(Label.TextProperty, static (HomePageViewModel vm) => vm.SelectedItemText);
            var layout = new Grid
            {
                Margin = new Thickness(20),
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto)
                }
            };
            layout.Add(new Label
            {
                Text = "Behaviors Demo",
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            }, 0, 0);
            layout.Add(listView, 0, 1);
            layout.Add(selectedItemLabel, 0, 2);
            Content = layout;
        }
    }
}
