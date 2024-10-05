namespace PlatformIntegrationDemo.ViewModels
{
    class ContactDetailsViewModel : BaseViewModel
    {
        public ContactDetailsViewModel(Contact contact)
        {
            Contact = contact;
        }

        public Contact Contact { get; }
    }
}

