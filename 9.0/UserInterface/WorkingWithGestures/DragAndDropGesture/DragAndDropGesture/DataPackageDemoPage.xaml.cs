namespace DragAndDropGesture
{
    public partial class DataPackageDemoPage : ContentPage
    {
        public DataPackageDemoPage()
        {
            InitializeComponent();
        }

        void OnMonkeyDragStarting(object sender, DragStartingEventArgs e)
        {
            e.Data.Text = "Monkey";
        }

        void OnCatDragStarting(object sender, DragStartingEventArgs e)
        {
            e.Data.Text = "Cat";
        }

        async void OnDrop(object sender, DropEventArgs e)
        {
            string text = await e.Data.GetTextAsync();

            if (text.Equals("Cat"))
            {
                await DisplayAlert("Correct! 🎉", "Well done! You successfully dragged the cat to the drop zone!", "OK");
            }
            else if (text.Equals("Monkey"))
            {
                await DisplayAlert("Try Again 🐵", "That's a monkey! Try dragging the cat instead.", "OK");
            }
            else
            {
                await DisplayAlert("Oops! 😅", "Something unexpected happened. Please try again.", "OK");
            }
        }
    }
}
