using CommunityToolkit.Mvvm.Messaging;

namespace DragAndDropGesture
{
    public class CustomDataPackageViewModel
    {
        Square draggedSquare;

        public ICommand DragStartingCommand => new Command<Square>(RegisterDragData);
        public ICommand DropCommand => new Command<Square>(ProcessDrop);

        void RegisterDragData(Square square)
        {
            draggedSquare = square;
        }

        void ProcessDrop(Square square)
        {
            if (square.Area.Equals(draggedSquare.Area))
            {
                WeakReferenceMessenger.Default.Send(new Message("Correct", "Congratulations!"));
            }
            else
            {
                WeakReferenceMessenger.Default.Send(new Message("Incorrect", "Try again."));
            }
        }
    }

    public class Message
    {
        public string Title { get; }
        public string Content { get; }

        public Message(string title, string content)
        {
            Title = title;
            Content = content;
        }
    }
}
