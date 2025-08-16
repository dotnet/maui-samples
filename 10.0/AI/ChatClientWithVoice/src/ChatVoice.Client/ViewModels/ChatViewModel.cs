using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ChatVoice.Client.Services;
using CommunityToolkit.Maui.Media;
using ChatVoice.Client.Models;

namespace ChatVoice.Client.ViewModels;

public partial class ChatViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private bool _isProcessing = false;

    public ObservableCollection<ChatMessage> Messages { get; } = new();

    private readonly IChatService _chatService;
    private readonly ILogger<ChatViewModel> _logger;

    public ChatViewModel(IChatService chatService, ILogger<ChatViewModel> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    [RelayCommand(CanExecute = nameof(CanSend))]
    private async Task Send()
    {
        var text = InputText?.Trim();
        if (string.IsNullOrEmpty(text) || IsProcessing) return;

        IsProcessing = true;
        try
        {
            Messages.Add(new ChatMessage { Text = text, From = Sender.User });
            InputText = string.Empty;
            var response = await _chatService.SendMessageAsync(text, Messages.ToList());
            Messages.Add(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Send failed");
            Messages.Add(new ChatMessage { Text = $"Error: {ex.Message}", From = Sender.Assistant, IsError = true });
        }
        finally { IsProcessing = false; }
    }

    [RelayCommand]
    private void ClearChat()
    {
        Messages.Clear();
        InputText = string.Empty;
    }

    [RelayCommand]
    private async Task SpeakOnline()
    {
        await RecognizeAsync(online: true);
    }

    [RelayCommand]
    private async Task SpeakOffline()
    {
        await RecognizeAsync(online: false);
    }

    private async Task RecognizeAsync(bool online)
    {
        try
        {
            var stt = online ? SpeechToText.Default : OfflineSpeechToText.Default;
            await stt.RequestPermissions(CancellationToken.None);

            string captured = string.Empty;
            stt.RecognitionResultUpdated += (sender, e) =>
            {
                var text = e?.RecognitionResult?.ToString();
                if (!string.IsNullOrWhiteSpace(text))
                    InputText = text!;
            };
            stt.RecognitionResultCompleted += (sender, e) =>
            {
                captured = e?.RecognitionResult?.ToString() ?? string.Empty;
            };

            await stt.StartListenAsync(new SpeechToTextOptions
            {
                Culture = System.Globalization.CultureInfo.CurrentCulture
            });
            // For sample simplicity, stop after 6 seconds if not already completed
            await Task.Delay(TimeSpan.FromSeconds(6));
            await stt.StopListenAsync();

            if (string.IsNullOrWhiteSpace(captured))
                captured = InputText;

            if (!string.IsNullOrWhiteSpace(captured))
            {
                InputText = captured;
                if (CanSend())
                    await Send();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Speech recognition failed");
            Messages.Add(new ChatMessage { Text = $"Error: {ex.Message}", From = Sender.Assistant, IsError = true });
        }
    }

    private bool CanSend() => !string.IsNullOrWhiteSpace(InputText) && !IsProcessing;

    [RelayCommand]
    private async Task SendPrompt(string? prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt)) return;
        InputText = prompt!;
        if (CanSend())
        {
            await Send();
        }
    }

    [RelayCommand]
    private async Task RefreshConfiguration()
    {
        try
        {
            _chatService.RefreshClient();
            var configured = await _chatService.IsConfiguredAsync();
            if (!configured)
                Messages.Add(new ChatMessage { Text = "⚠️ Credentials not configured. Use Setup tab to load them.", From = Sender.Assistant });
            else
                Messages.Add(new ChatMessage { Text = "✅ Chat configuration refreshed.", From = Sender.Assistant });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh configuration");
            Messages.Add(new ChatMessage { Text = $"Error: {ex.Message}", From = Sender.Assistant, IsError = true });
        }
    }
}
