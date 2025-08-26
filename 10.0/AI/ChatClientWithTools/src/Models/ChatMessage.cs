using System.Text.Json;

namespace ChatClientWithTools.Models;

public enum Sender
{
    User,
    Assistant
}

public enum MessageType
{
    Text,
    ToolCall,
    ToolResult
}

public class ChatMessage
{
    public required string Text { get; set; }
    public required Sender From { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
    public MessageType Type { get; set; } = MessageType.Text;
    public string? ToolName { get; set; }
    public JsonDocument? ToolParameters { get; set; }
    public object? ToolResult { get; set; }
    public bool IsError { get; set; }

    // Helper properties for UI binding
    public bool IsToolCall => Type == MessageType.ToolCall;
    public bool IsToolResult => Type == MessageType.ToolResult;
    public bool IsPlainText => Type == MessageType.Text;
    public bool IsWeatherResult => IsToolResult && ToolName == "get_weather";
    public bool IsCalculatorResult => IsToolResult && ToolName == "calculate";
    public bool IsFileResult => IsToolResult && ToolName == "list_files";
    public bool IsSystemInfoResult => IsToolResult && ToolName == "get_system_info";
    public bool IsTimerResult => IsToolResult && ToolName == "set_timer";

    // Typed result accessors
    public WeatherResult? AsWeatherResult => ToolResult as WeatherResult;
    public CalculationResult? AsCalculationResult => ToolResult as CalculationResult;
    public FileListResult? AsFileListResult => ToolResult as FileListResult;
    public SystemInfoResult? AsSystemInfoResult => ToolResult as SystemInfoResult;
    public TimerResult? AsTimerResult => ToolResult as TimerResult;
}

// Tool result types
public class WeatherResult
{
    public required string Location { get; set; }
    public required double Temperature { get; set; }
    public required string TemperatureUnit { get; set; }
    public required string Description { get; set; }
    public required string Condition { get; set; }
    public required double Humidity { get; set; }
    public required double WindSpeed { get; set; }
    public string? Icon { get; set; }
}

public class CalculationResult
{
    public required string Expression { get; set; }
    public required string Result { get; set; }
    public string? Steps { get; set; }
}

public class FileListResult
{
    public required string Path { get; set; }
    public required List<FileInfo> Files { get; set; }
    public class FileInfo
    {
        public required string Name { get; set; }
        public required string FullPath { get; set; }
        public required long Size { get; set; }
        public required DateTimeOffset Modified { get; set; }
        public required bool IsDirectory { get; set; }
        public string Icon => IsDirectory ? "📁" : GetFileIcon();
        
        private string GetFileIcon()
        {
            var extension = System.IO.Path.GetExtension(Name).ToLower();
            return extension switch
            {
                ".txt" => "📄",
                ".pdf" => "📕",
                ".doc" or ".docx" => "📘",
                ".xls" or ".xlsx" => "📗",
                ".ppt" or ".pptx" => "📙",
                ".jpg" or ".jpeg" or ".png" or ".gif" => "🖼️",
                ".mp4" or ".avi" or ".mov" => "🎬",
                ".mp3" or ".wav" => "🎵",
                ".zip" or ".rar" => "📦",
                ".exe" => "⚙️",
                _ => "📄"
            };
        }
    }
}

public class SystemInfoResult
{
    public required double BatteryLevel { get; set; }
    public required string BatteryState { get; set; }
    public required long AvailableStorage { get; set; }
    public required long TotalStorage { get; set; }
    public required long AvailableMemory { get; set; }
    public required long TotalMemory { get; set; }
    public required string DeviceName { get; set; }
    public required string Platform { get; set; }
    public required string Version { get; set; }
}

public class TimerResult
{
    public required int DurationMinutes { get; set; }
    public required string Title { get; set; }
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    public required string TimerId { get; set; }
}