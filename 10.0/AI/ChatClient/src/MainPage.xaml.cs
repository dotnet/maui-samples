using SimpleChatClient.ViewModels;

namespace SimpleChatClient;

public partial class MainPage : ContentPage
{
	public MainPage() : this(
		Application.Current?.Handler?.MauiContext?.Services?.GetRequiredService<ChatViewModel>()
		?? throw new InvalidOperationException("ChatViewModel not registered"))
	{ }

	public MainPage(ChatViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;

		// Auto-scroll to bottom when messages change
		vm.Messages.CollectionChanged += (_, __) =>
		{
			if (MessagesView.ItemsSource is not null)
			{
				// Scroll to the last item
				var last = vm.Messages.LastOrDefault();
				if (last is not null)
					MessagesView.ScrollTo(last, position: ScrollToPosition.End, animate: true);
			}
		};
	}

	// Enter key from Editor triggers Send
	private void OnCompleted(object? sender, EventArgs e)
	{
		if (BindingContext is ChatViewModel vm && vm.SendCommand.CanExecute(null))
		{
			vm.SendCommand.Execute(null);
		}
	}

	private void OnTextChanged(object? sender, TextChangedEventArgs e)
	{
		// If user presses Enter, many platforms insert '\n' into the Editor.
		// When we detect trailing newline and there's text before it, submit.
		var text = e.NewTextValue ?? string.Empty;
		if (text.EndsWith("\n", StringComparison.Ordinal))
		{
			if (BindingContext is ChatViewModel vm)
			{
				vm.InputText = text.TrimEnd('\r', '\n');
				if (vm.SendCommand.CanExecute(null))
					vm.SendCommand.Execute(null);
			}
		}
	}
}
