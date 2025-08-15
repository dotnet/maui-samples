

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
                DisplayAlert("Correct", e.ToString(), "OK");
            });

            WeakReferenceMessenger.Default.Register<CustomDataPackageViewModel, string>(this, "Incorrect", (s, e) =>
            {
                DisplayAlert("Incorrect", e.ToString(), "OK");
            });
        }
    }
}
