using LocalChatClientWithTools.ViewModels;

namespace LocalChatClientWithTools;

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
}