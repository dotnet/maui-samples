using ChatMobile.Client.ViewModels;

namespace ChatMobile.Client.Views;

public partial class MainPage : ContentPage
{
    public MainPage(ChatViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnCompleted(object sender, EventArgs e)
    {
        var viewModel = (ChatViewModel)BindingContext;
        if (viewModel.SendCommand.CanExecute(null))
        {
            await viewModel.SendCommand.ExecuteAsync(null);
        }
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        // Keep for parity with provided snippet; no-op is fine or hook debounced typing here.
    }
}