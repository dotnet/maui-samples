using Passkeys.Client.ViewModels;

namespace Passkeys.Client;

public partial class MainPage : ContentPage
{
    public MainPage(PasskeysViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
