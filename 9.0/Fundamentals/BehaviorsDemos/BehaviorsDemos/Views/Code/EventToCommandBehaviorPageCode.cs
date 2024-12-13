namespace BehaviorsDemos
{
    public class EventToCommandBehaviorPageCode : ContentPage
    {
        public EventToCommandBehaviorPageCode()
        {
            BindingContext = new HomePageViewModel();

            var listView = new ListView();
            listView.SetBinding(ItemsView<Cell>.ItemsSourceProperty, static (HomePageViewModel vm) => vm.People);
            listView.ItemTemplate = new DataTemplate(() =>
            {
                var textCell = new TextCell();
                textCell.SetBinding(TextCell.TextProperty, static (Person person) => person.Name);
                return textCell;
            });
            listView.Behaviors.Add(new EventToCommandBehavior
            {
                EventName = "ItemSelected",
                Command = ((HomePageViewModel)BindingContext).OutputAgeCommand,
                Converter = new SelectedItemEventArgsToSelectedItemConverter()
            });

            var selectedItemLabel = new Label();
            selectedItemLabel.SetBinding(Label.TextProperty, static (HomePageViewModel vm) => vm.SelectedItemText);
            Content = new StackLayout
            {
                Margin = new Thickness(20),
                Children = {
                    new Label {
                        Text = "Behaviors Demo",
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center
                    },
                    listView,
                    selectedItemLabel
                }
            };
        }
    }
}
