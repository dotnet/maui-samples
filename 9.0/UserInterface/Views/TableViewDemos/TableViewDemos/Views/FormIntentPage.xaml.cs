using System.Collections.ObjectModel;
using TableViewDemos.Models;

namespace TableViewDemos
{
	public partial class FormIntentPage : ContentPage
	{
		public ObservableCollection<FormSection> Sections { get; } = new();

		public FormIntentPage()
		{
			InitializeComponent();
			BindingContext = this;

			// Initialize form sections and fields
			Sections.Add(new FormSection
			{
				Title = "Login details",
				Fields = new List<FormField>
				{
					new FormField { Label = "Login", Placeholder = "username" },
					new FormField { Label = "Password", Placeholder = "password", IsPassword = true }
				}
			});

			Sections.Add(new FormSection
			{
				Title = "Contact details",
				Fields = new List<FormField>
				{
					new FormField { Label = "Name", Placeholder = "name" },
					new FormField { Label = "Address", Placeholder = "address" },
					new FormField { Label = "Phone number", Placeholder = "mobile number" }
				}
			});
		}
	}
}

