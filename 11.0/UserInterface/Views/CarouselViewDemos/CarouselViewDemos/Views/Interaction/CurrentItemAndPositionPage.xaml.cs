using CarouselViewDemos.Models;

namespace CarouselViewDemos.Views
{
    public partial class CurrentItemAndPositionPage : ContentPage
    {
        Monkey previousItem;
        Monkey currentItem;
        int previousItemPosition;
        int currentItemPosition;

        public CurrentItemAndPositionPage()
        {
            InitializeComponent();
            UpdateLabels();
        }

        void OnCurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            previousItem = e.PreviousItem as Monkey;
            currentItem = e.CurrentItem as Monkey;
            UpdateLabels();
        }

        void OnPositionChanged(object sender, PositionChangedEventArgs e)
        {
            previousItemPosition = e.PreviousPosition;
            currentItemPosition = e.CurrentPosition;
            UpdateLabels();
        }

        void UpdateLabels()
        {
            previousItemLabel.Text = previousItem?.Name ?? "None";
            currentItemLabel.Text = currentItem?.Name ?? "None";
            previousItemPositionLabel.Text = previousItemPosition.ToString();
            currentItemPositionLabel.Text = currentItemPosition.ToString();
        }
    }
}
