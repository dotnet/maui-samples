using System.Collections.ObjectModel;
using System.Windows.Input;
using TableViewDemos.Models;

namespace TableViewDemos
{
	public partial class DataIntentPage : ContentPage
	{
		public ObservableCollection<DataItem> Items { get; } = new();
		public ICommand ToggleExpandCommand { get; }

		public DataIntentPage ()
		{
			InitializeComponent ();
			BindingContext = this;

			// Initialize the command
			ToggleExpandCommand = new Command<DataItem>(ToggleExpand);

			// Add sample items
			Items.Add(new DataItem 
			{ 
				Text = "This CollectionView replaces the TableView data intent.",
				Detail = "Data is better presented in a CollectionView.",
				IsExpandable = false
			});

			Items.Add(new DataItem
			{
				Text = "Images can also be displayed.",
				Detail = "The layout adjusts automatically.",
				ImageSource = "dotnet_bot.png",
				IsExpandable = false
			});

			Items.Add(new DataItem
			{
				Text = "Tap this cell.",
				Detail = "It will expand and collapse.",
				IsExpandable = true
			});
		}

		private void ToggleExpand(DataItem item)
		{
			if (item.IsExpandable)
			{
				item.IsExpanded = !item.IsExpanded;
				// Force CollectionView to refresh this item
				var index = Items.IndexOf(item);
				Items.RemoveAt(index);
				Items.Insert(index, item);
			}
		}
	}
}
