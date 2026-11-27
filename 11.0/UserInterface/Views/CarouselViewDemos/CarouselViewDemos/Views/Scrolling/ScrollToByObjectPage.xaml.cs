using CarouselViewDemos.Models;
using CarouselViewDemos.ViewModels;
using System.Diagnostics;

namespace CarouselViewDemos.Views
{
    public partial class ScrollToByObjectPage : ContentPage
    {
        public ScrollToByObjectPage()
        {
            InitializeComponent();
        }

        void OnButtonClicked(object sender, EventArgs e)
        {
            MonkeysViewModel viewModel = BindingContext as MonkeysViewModel;
            Monkey monkey = viewModel.Monkeys.FirstOrDefault(m => m.Name == "Proboscis Monkey");
            // Known issue: while Loop is true (the default), ScrollTo(item) doesn't reach the item on iOS and Android.
            // ScrollTo(index) and Loop="False" work. See https://github.com/dotnet/maui/issues/40761
            carouselView.ScrollTo(monkey, position: (ScrollToPosition)enumPicker.SelectedItem, animate: animateSwitch.IsToggled);
        }

        void OnCarouselViewScrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            Debug.WriteLine("HorizontalDelta: " + e.HorizontalDelta);
            Debug.WriteLine("VerticalDelta: " + e.VerticalDelta);
            Debug.WriteLine("HorizontalOffset: " + e.HorizontalOffset);
            Debug.WriteLine("VerticalOffset: " + e.VerticalOffset);
            Debug.WriteLine("FirstVisibleItemIndex: " + e.FirstVisibleItemIndex);
            Debug.WriteLine("CenterItemIndex: " + e.CenterItemIndex);
            Debug.WriteLine("LastVisibleItemIndex: " + e.LastVisibleItemIndex);
        }
    }
}
