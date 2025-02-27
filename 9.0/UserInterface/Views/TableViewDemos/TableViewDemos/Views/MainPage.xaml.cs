using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace TableViewDemos
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<MenuItem> MenuItems { get; set; }
        public Command<Type> NavigateCommand { get; set; }

        public MainPage()
        {
            InitializeComponent();
            MenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem { Text = "Menu page", PageType = typeof(MenuIntentPage) },
                new MenuItem { Text = "Settings page", PageType = typeof(SettingsIntentPage) },
                new MenuItem { Text = "Form page", PageType = typeof(FormIntentPage) },
                new MenuItem { Text = "Data page", PageType = typeof(DataIntentPage) },
                new MenuItem { Text = "ImageCell page", PageType = typeof(ImageCellPage) },
                new MenuItem { Text = "Right to left TableView", PageType = typeof(RightToLeftTablePage) }
            };
            NavigateCommand = new Command<Type>(async (pageType) => await NavigateToPage(pageType));
            BindingContext = this;
        }

        private async Task NavigateToPage(Type pageType)
        {
            var page = (Page)Activator.CreateInstance(pageType);
            await Navigation.PushAsync(page);
        }
    }

    public class MenuItem
    {
        public string Text { get; set; }
        public Color TextColor { get; set; } = Colors.Black;
        public Type PageType { get; set; }
    }
}
