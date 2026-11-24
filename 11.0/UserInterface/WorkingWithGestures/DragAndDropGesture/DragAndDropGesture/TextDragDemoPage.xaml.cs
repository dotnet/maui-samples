namespace DragAndDropGesture
{
    public partial class TextDragDemoPage : ContentPage
    {
        public TextDragDemoPage()
        {
            InitializeComponent();
        }

        async void OnDropGestureRecognizerDrop(object sender, DropEventArgs e)
        {
            string droppedText = await e.Data.GetTextAsync();
            
            if (droppedText == "4")
            {
                await DisplayAlertAsync("Correct! 🎉", "Great job! 2 + 2 = 4", "OK");
            }
            else if (droppedText == "3")
            {
                await DisplayAlertAsync("Try Again 🤔", "Not quite right. 2 + 2 = 4, not 3. Give it another try!", "OK");
            }
            else
            {
                await DisplayAlertAsync("Oops! 😅", "Something went wrong. Please try dragging an answer again.", "OK");
            }
        }

        void OnDropGestureRecognizerDragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }
    }
}
