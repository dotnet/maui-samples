using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace ModalNavigation;

	public partial class MainPage : ContentPage
	{
		List<Contact> contacts;

		public MainPage ()
		{
			InitializeComponent ();

			SetupData ();
			collectionView.ItemsSource = contacts;
		}

		async void OnSelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			if (e.CurrentSelection.FirstOrDefault() is Contact contact) {
				var detailPage = new DetailPage ();
				detailPage.BindingContext = contact;
				collectionView.SelectedItem = null;
				await Navigation.PushModalAsync (detailPage);
			}
		}

		void SetupData ()
		{
			contacts = new List<Contact> ();
			contacts.Add (new Contact {
				Name = "Jane Doe",
				Age = 30,
				Occupation = "Developer",
				Country = "USA"
			});
			contacts.Add (new Contact {
				Name = "John Doe",
				Age = 34,
				Occupation = "Tester",
				Country = "USA"
			});
			contacts.Add (new Contact {
				Name = "John Smith",
				Age = 52,
				Occupation = "PM",
				Country = "UK"
			});
			contacts.Add (new Contact {
				Name = "Kath Smith",
				Age = 55,
				Occupation = "Business Analyst",
				Country = "UK"
			});
			contacts.Add (new Contact {
				Name = "Steve Smith",
				Age = 19,
				Occupation = "Junior Developer",
				Country = "UK"
			});
		}
	}


