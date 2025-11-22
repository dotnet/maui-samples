using System.Collections.ObjectModel;

namespace SafeAreaDemos.ViewModels;

public class Example4ViewModel
{
    public ObservableCollection<ContactItem> Contacts { get; set; }

    public Example4ViewModel()
    {
        Contacts = new ObservableCollection<ContactItem>
        {
            new ContactItem { Section = "TODAY", IsSection = true },
            new ContactItem { Name = "Lisa", IsContact = true },
            new ContactItem { Name = "Alice", IsContact = true },
            new ContactItem { Name = "Tina", IsContact = true },
            new ContactItem { Name = "Lee", IsContact = true },

            new ContactItem { Section = "YESTERDAY", IsSection = true },
            new ContactItem { Name = "Pete", IsContact = true },
            new ContactItem { Name = "Diana", IsContact = true },
            new ContactItem { Name = "Sam", IsContact = true },
            new ContactItem { Name = "Maya", IsContact = true },

            new ContactItem { Section = "WEDNESDAY", IsSection = true },
            new ContactItem { Name = "Jack", IsContact = true },
            new ContactItem { Name = "Emma", IsContact = true },
            new ContactItem { Name = "Noah", IsContact = true },
            new ContactItem { Name = "Olivia", IsContact = true },
            new ContactItem { Name = "Liam", IsContact = true }
        };
    }
}

public class ContactItem
{
    public string Section { get; set; }
    public bool IsSection { get; set; }
    public string Name { get; set; }
    public bool IsContact { get; set; }
}
