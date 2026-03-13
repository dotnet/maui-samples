using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LocalChatClientWithTools.ViewModels;

public partial class ToolCallMessageViewModel : ChatMessageViewModel
{
    public required string ToolName { get; init; }

    [ObservableProperty]
    private string _text = string.Empty;

    [ObservableProperty]
    private bool _isExpanded;

    [ObservableProperty]
    private string? _rawJson;

    public bool IsCompleted { get; set; }

    public bool HasRawJson => RawJson is not null;

    [RelayCommand]
    void ToggleExpanded() => IsExpanded = !IsExpanded;
}
