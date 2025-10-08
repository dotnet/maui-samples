namespace ChatMobile.Client.Models;

public class ChatRequest
{
    public required string Message { get; set; }
    public List<string> ConversationHistory { get; set; } = new();
}

public class ChatResponse
{
    public required string Message { get; set; }
    public List<ClientToolRequest> ClientToolRequests { get; set; } = new();
    public List<ToolResult> ServerToolResults { get; set; } = new();
    public bool IsError { get; set; }
    public string? ErrorMessage { get; set; }
}

public class ClientToolRequest
{
    public required string ToolName { get; set; }
    public required string RequestId { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class ToolExecutionRequest
{
    public required string RequestId { get; set; }
    public required string ToolName { get; set; }
    public required object Result { get; set; }
    public string? OriginalMessage { get; set; }
    public List<string> ConversationHistory { get; set; } = new();
}

public class ToolResult
{
    public required string ToolName { get; set; }
    public required object Result { get; set; }
    public bool IsError { get; set; }
    public string? ErrorMessage { get; set; }
}