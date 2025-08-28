using CommunityToolkit.Mvvm.Messaging;

namespace DragAndDropGesture
{
    public class DataPackageViewModel
    {
        string dragData;

        public ICommand DragCommand => new Command<string>(RegisterDragData);
        public ICommand DropCommand => new Command(ProcessDrop);

        void RegisterDragData(string data)
        {
            dragData = data;
        }

        void ProcessDrop()
        {
            if (dragData.Equals("Cat"))
            {
                WeakReferenceMessenger.Default.Send(new Message("Correct", "Congratulations!"));
            }
            else if (dragData.Equals("Monkey"))
            {
                WeakReferenceMessenger.Default.Send(new Message("Incorrect", "Try again."));
            }
        }
    }
}
