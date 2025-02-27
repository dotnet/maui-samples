using System.Collections.ObjectModel;
using System.Windows.Input;

namespace TableViewDemos
{
    public partial class DataIntentPage : ContentPage
    {
        public ObservableCollection<DataItem> Items { get; set; }
        public ICommand ToggleExpandCommand { get; private set; }

        public DataIntentPage()
        {
            InitializeComponent();

            // Initialize data items
            Items = new ObservableCollection<DataItem>
            {
                new DataItem 
                { 
                    Text = "This TableView uses the data intent.",
                    Detail = "However, data is often better presented in a ListView.",
                    HasDetail = true
                },
                new DataItem 
                { 
                    Text = "Images can also be displayed.",
                    Detail = "HasUnevenRows is set to true.",
                    HasDetail = true,
                    HasImage = true,
                    ImageSource = "dotnet_bot.png"
                },
                new DataItem 
                { 
                    Text = "Tap this cell.",
                    ExtraText = "The cell has changed size.",
                    HasDetail = false
                }
            };

            // Command to handle item tapping
            ToggleExpandCommand = new Command<DataItem>(item =>
            {
                if (item.Text == "Tap this cell.")
                {
                    item.IsExpanded = !item.IsExpanded;
                }
            });

            // Set the binding context
            BindingContext = this;
            collectionView.ItemsSource = Items;
        }
    }

    public class DataItem
    {
        public string Text { get; set; }
        public string Detail { get; set; }
        public bool HasDetail { get; set; }
        public string ImageSource { get; set; }
        public bool HasImage { get; set; }
        public string ExtraText { get; set; }
        public bool IsExpanded { get; set; }
        public int ExtraTextColumn => HasImage ? 1 : 0;
    }
}
