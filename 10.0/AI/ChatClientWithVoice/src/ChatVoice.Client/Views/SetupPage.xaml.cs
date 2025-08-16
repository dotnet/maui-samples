using ChatVoice.Client.ViewModels;

namespace ChatVoice.Client.Views;

public partial class SetupPage : ContentPage
{
    public SetupPage(SetupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
