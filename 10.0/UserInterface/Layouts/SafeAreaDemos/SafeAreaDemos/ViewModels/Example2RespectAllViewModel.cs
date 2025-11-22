using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SafeAreaDemos.ViewModels;

public class Example2RespectAllViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Contact> Contacts { get; set; }
    public ObservableCollection<Contact> FilteredContacts { get; set; }

    public Example2RespectAllViewModel()
    {
        Contacts = new ObservableCollection<Contact>
        {
            new Contact { Name = "Lisa" },
            new Contact { Name = "Alice" },
            new Contact { Name = "Tina" },
            new Contact { Name = "Lee" },
            new Contact { Name = "Pete" },
            new Contact { Name = "Diana" },
            new Contact { Name = "Sam" },
            new Contact { Name = "Maya" },
            new Contact { Name = "Jack" },
            new Contact { Name = "Emma" },
            new Contact { Name = "Noah" },
            new Contact { Name = "Olivia" },
            new Contact { Name = "Liam" },
            new Contact { Name = "Sophia" },
            new Contact { Name = "Ava" },
            new Contact { Name = "Isabella" },
            new Contact { Name = "Mia" },
            new Contact { Name = "Charlotte" },
            new Contact { Name = "Amelia" },
            new Contact { Name = "Harper" },
            new Contact { Name = "Evelyn" },
            new Contact { Name = "Abigail" },
            new Contact { Name = "Emily" },
            new Contact { Name = "Luna" },
            new Contact { Name = "Ella" },
            new Contact { Name = "Aria" },
            new Contact { Name = "Scarlett" },
            new Contact { Name = "Chloe" },
            new Contact { Name = "Grace" }
        };

        FilteredContacts = new ObservableCollection<Contact>(Contacts);
    }

    public void FilterContacts(string searchText)
    {
        var search = searchText?.ToLower() ?? string.Empty;

        FilteredContacts.Clear();

        if (string.IsNullOrWhiteSpace(search))
        {
            foreach (var contact in Contacts)
            {
                FilteredContacts.Add(contact);
            }
        }
        else
        {
            foreach (var contact in Contacts)
            {
                if (contact.Name.ToLower().Contains(search))
                {
                    FilteredContacts.Add(contact);
                }
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class Contact
{
    public string Avatar { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsLastItem { get; set; }
}
