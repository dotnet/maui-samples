

using CommunityToolkit.Mvvm.Messaging;

namespace DragAndDropGesture
{
    public partial class CustomDataPackageCommandDemoPage : ContentPage
    {
        public CustomDataPackageCommandDemoPage()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<CustomDataPackageViewModel, string>(this, "Correct", (s, e) =>
            {
                DisplayAlertAsync("Correct", e.ToString(), "OK");
            });

            WeakReferenceMessenger.Default.Register<CustomDataPackageViewModel, string>(this, "Incorrect", (s, e) =>
            {
                DisplayAlertAsync("Incorrect", e.ToString(), "OK");
            });
        }
    }
}
