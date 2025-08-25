namespace ModalNavigation;

public class MainPageCS : ContentPage
{
    private ListView listView;
    private List<Contact> contacts = [];

    public MainPageCS()
    {
        SetupData();

        listView = new ListView
        {
            ItemsSource = contacts
        };
        listView.ItemSelected += OnItemSelected;

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
                    listView
                }
        };
    }

    async void OnItemSelected(object? sender, SelectedItemChangedEventArgs e)
    {
        if (listView.SelectedItem != null)
        {
            var detailPage = new DetailPageCS
            {
                BindingContext = e.SelectedItem as Contact
            };
            listView.SelectedItem = null;
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

