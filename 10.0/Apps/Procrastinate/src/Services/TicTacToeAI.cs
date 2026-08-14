using Microsoft.Extensions.AI;

namespace procrastinate.Services;

/// <summary>
/// Uses Cloud AI or On-Device AI (via MEAI IChatClient) to play TicTacToe
/// </summary>
public class TicTacToeAI
{
    private readonly IChatClient? _onDeviceClient;
    private IChatClient? _cloudClient;

    public TicTacToeAI(IChatClient? onDeviceClient = null)
    {
        _onDeviceClient = onDeviceClient;
    }

    /// <summary>
    /// Last AI conversation for debugging display
    /// </summary>
    public string LastDebugInfo { get; private set; } = "";

    private static string ApiKey => CloudExcuseGenerator.GetApiKey();
    private static string Model => Preferences.Get("GroqModel", "llama-3.3-70b-versatile");

    /// <summary>
    /// Check if AI is available (cloud API key configured or on-device AI available)
    /// </summary>
    public bool IsAvailable => _onDeviceClient is not null || !string.IsNullOrEmpty(ApiKey);

    /// <summary>
    /// Get AI's move for TicTacToe. Returns -1 if AI fails.
    /// </summary>
    public async Task<int> GetMoveAsync(string[] board)
    {
        LastDebugInfo = "";

        // Try on-device AI first
        if (_onDeviceClient is not null)
        {
            var move = await GetMoveFromClientAsync(_onDeviceClient, board, "🍎 Apple Intelligence");
            if (move >= 0) return move;
        }

        // Fall back to cloud AI (reuse client across moves)
        if (!string.IsNullOrEmpty(ApiKey))
        {
            try
            {
                _cloudClient ??= CloudExcuseGenerator.CreateChatClient();
                var move = await GetMoveFromClientAsync(_cloudClient, board, $"☁️ {Model}");
                if (move >= 0) return move;
            }
            catch (Exception ex)
            {
                LastDebugInfo = $"☁️ Cloud AI · ❌ {ex.Message}";
                // Reset client on error so next call creates a fresh one
                (_cloudClient as IDisposable)?.Dispose();
                _cloudClient = null;
            }
        }

        LastDebugInfo = "⚙️ Built-in strategy";
        return -1;
    }

    private async Task<int> GetMoveFromClientAsync(IChatClient client, string[] board, string label)
    {
        try
        {
            var boardStr = FormatBoard(board);
            var prompt = $"TicTacToe as O. Board:\n{boardStr}\nReply with ONE digit (0-8) for empty (.) position.";

            var messages = new List<ChatMessage>
            {
                new(ChatRole.User, prompt)
            };

            var response = await client.GetResponseAsync(messages);
            var content = response.Text?.Trim() ?? "";

            foreach (char c in content)
            {
                if (char.IsDigit(c))
                {
                    int move = c - '0';
                    if (move >= 0 && move <= 8 && string.IsNullOrEmpty(board[move]))
                    {
                        LastDebugInfo = $"{label} · ✓ \"{content}\"";
                        return move;
                    }
                }
            }
            LastDebugInfo = $"{label} · ⚠️ \"{content}\" invalid";
        }
        catch (Exception ex)
        {
            LastDebugInfo = $"{label} · ❌ {ex.Message}";
        }

        return -1;
    }

    private static string FormatBoard(string[] board)
    {
        string Cell(int i) => string.IsNullOrEmpty(board[i]) ? "." : board[i];
        return $"{Cell(0)}|{Cell(1)}|{Cell(2)}\n{Cell(3)}|{Cell(4)}|{Cell(5)}\n{Cell(6)}|{Cell(7)}|{Cell(8)}";
    }
}
