namespace ModalNavigation;

public class MainPageCS : ContentPage
{
    private CollectionView collectionView;
    private List<Contact> contacts = [];

    public MainPageCS()
    {
        SetupData();

        collectionView = new CollectionView
        {
            ItemsSource = contacts
        };
        collectionView.SelectionChanged += OnSelectionChanged;

        Thickness padding;
        if (DeviceInfo.Platform.Equals(DevicePlatform.iOS))
        {
            padding = new Thickness(0, 40, 0, 0);
        }
        else
        {
            padding = new Thickness();
        }

        Padding = padding;
        Content = new StackLayout
        {
            Children = {
                    collectionView
                }
        };
    }

    async void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Contact contact)
        {
            var detailPage = new DetailPageCS
            {
                BindingContext = contact
            };
            collectionView.SelectedItem = null;
            await Navigation.PushModalAsync(detailPage);
        }
    }

    void SetupData()
    {
        contacts =
        [
            new Contact
            {
                Name = "Jane Doe",
                Age = 30,
                Occupation = "Developer",
                Country = "USA"
            },
            new Contact
            {
                Name = "John Doe",
                Age = 34,
                Occupation = "Tester",
                Country = "USA"
            },
            new Contact
            {
                Name = "John Smith",
                Age = 52,
                Occupation = "PM",
                Country = "UK"
            },
            new Contact
            {
                Name = "Kath Smith",
                Age = 55,
                Occupation = "Business Analyst",
                Country = "UK"
            },
            new Contact
            {
                Name = "Steve Smith",
                Age = 19,
                Occupation = "Junior Developer",
                Country = "UK"
            },
        ];
    }
}

