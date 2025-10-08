using ChatMobile.Client.ViewModels;

namespace ChatMobile.Client.Views;

public partial class SetupPage : ContentPage
{
    public SetupPage(SetupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}