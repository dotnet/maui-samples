using ChatClientWithTools.ViewModels;

namespace ChatClientWithTools;

public partial class MainPage : ContentPage
{
    public MainPage(ChatViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnCompleted(object? sender, EventArgs e)
    {
        var viewModel = BindingContext as ChatViewModel;
        if (viewModel?.SendCommand.CanExecute(null) == true)
        {
            await viewModel.SendCommand.ExecuteAsync(null);
        }
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        // This will trigger CanExecute updates for the SendCommand
        var viewModel = BindingContext as ChatViewModel;
        viewModel?.SendCommand.NotifyCanExecuteChanged();
    }
}