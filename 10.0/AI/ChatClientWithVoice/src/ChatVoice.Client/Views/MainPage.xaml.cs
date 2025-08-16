using Microsoft.Maui.Controls;

namespace ChatVoice.Client.Views;

using ChatVoice.Client.ViewModels;

public partial class MainPage : ContentPage
{
    public MainPage(ChatViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void OnCompleted(object sender, EventArgs e)
    {
        if (BindingContext is ChatVoice.Client.ViewModels.ChatViewModel vm && vm.SendCommand.CanExecute(null))
        {
            vm.SendCommand.Execute(null);
        }
    }
}
