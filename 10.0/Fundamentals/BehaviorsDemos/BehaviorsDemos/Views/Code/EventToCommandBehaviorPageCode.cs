namespace BehaviorsDemos
{
    public class EventToCommandBehaviorPageCode : ContentPage
    {
        public EventToCommandBehaviorPageCode()
        {
            BindingContext = new HomePageViewModel();

            var collectionView = new CollectionView
            {
                SelectionMode = SelectionMode.Single
            };
            collectionView.SetBinding(ItemsView.ItemsSourceProperty, static (HomePageViewModel vm) => vm.People);
            collectionView.ItemTemplate = new DataTemplate(() =>
            {
                var nameLabel = new Label { Padding = 10 };
                nameLabel.SetBinding(Label.TextProperty, static (Person person) => person.Name);

                var divider = new BoxView
                {
                    Background = Colors.LightGray,
                    HeightRequest = 1
                };

                var itemLayout = new Grid
                {
                    RowDefinitions =
                    {
                        new RowDefinition(GridLength.Auto),
                        new RowDefinition(1)
                    }
                };
                itemLayout.Add(nameLabel);
                itemLayout.Add(divider, row: 1);
                return itemLayout;
            });
            collectionView.Behaviors.Add(new EventToCommandBehavior
            {
                EventName = "SelectionChanged",
                Command = ((HomePageViewModel)BindingContext).OutputAgeCommand,
                Converter = new SelectionChangedEventArgsToSelectedItemConverter()
            });

            var selectedItemLabel = new Label();
            selectedItemLabel.SetBinding(Label.TextProperty, static (HomePageViewModel vm) => vm.SelectedItemText);
            var layout = new Grid
            {
                Margin = new Thickness(20),
                RowDefinitions =
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
            });
            layout.Add(collectionView, row: 1);
            layout.Add(selectedItemLabel, row: 2);
            Content = layout;
        }
    }
}
