namespace DragAndDropGesture
{
    public partial class ImageDragDemoPage : ContentPage
    {
        public ImageDragDemoPage()
        {
            InitializeComponent();
        }

        async void OnCorrectDrop(object sender, DropEventArgs e)
        {
            await DisplayAlert("Correct! 🎉", "Excellent! You correctly identified the monkey and dropped it in the right category!", "OK");
        }

        void OnIncorrectDragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.None;
            // Show a brief visual feedback that this drop zone won't accept the drop
            DisplayAlert("Not Quite! 🐱", "This is the cat category, but you're dragging a monkey. Try the other category!", "OK");
        }
    }
}
