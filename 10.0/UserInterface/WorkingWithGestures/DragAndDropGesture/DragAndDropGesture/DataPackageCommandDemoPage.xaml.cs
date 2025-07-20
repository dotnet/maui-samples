

using CommunityToolkit.Mvvm.Messaging;

namespace DragAndDropGesture
{
    public partial class DataPackageCommandDemoPage : ContentPage
    {
        public DataPackageCommandDemoPage()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<DataPackageViewModel, string>(this, "Correct", (s, e) =>
            {
                DisplayAlert("Correct", e.ToString(), "OK");
            });

            WeakReferenceMessenger.Default.Register<DataPackageViewModel, string>(this, "Incorrect", (s, e) =>
            {
                DisplayAlert("Incorrect", e.ToString(), "OK");
            });
        }
    }
}
