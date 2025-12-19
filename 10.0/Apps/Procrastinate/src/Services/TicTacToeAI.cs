using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace procrastinate.Services;

/// <summary>
/// Uses Cloud AI or On-Device AI to play TicTacToe
/// </summary>
public static class TicTacToeAI
{
    private static string ApiKey => Preferences.Get("GroqApiKey", "");
    private static string ApiEndpoint => Preferences.Get("GroqApiEndpoint", "https://api.groq.com/openai/v1/chat/completions");
    private static string Model => Preferences.Get("GroqModel", "llama-3.3-70b-versatile");
    
    private static readonly Lazy<HttpClient> _httpClient = new(() => 
        new HttpClient { Timeout = TimeSpan.FromSeconds(10) });

    /// <summary>
    /// Last AI conversation for debugging display
    /// </summary>
    public static string LastDebugInfo { get; private set; } = "";

    /// <summary>
    /// Check if AI is available (cloud API key configured or on-device AI available)
    /// </summary>
    public static bool IsAvailable
    {
        get
        {
            if (!string.IsNullOrEmpty(ApiKey)) return true;
            
#if IOS || MACCATALYST
            try
            {
                var onDevice = new OnDeviceAIExcuseGenerator();
                return onDevice.IsAvailable;
            }
            catch
            {
                return false;
            }
#else
            return false;
#endif
        }
    }

    /// <summary>
    /// Get AI's move for TicTacToe. Returns -1 if AI fails.
    /// </summary>
    /// <param name="board">Array of 9 strings: "X", "O", or "" for empty</param>
    /// <returns>Index 0-8 for the move, or -1 on failure</returns>
    public static async Task<int> GetMoveAsync(string[] board)
    {
        LastDebugInfo = "";
        
#if IOS || MACCATALYST
        // Try on-device AI first if available (Apple platforms only)
        try
        {
            var onDevice = new OnDeviceAIExcuseGenerator();
            if (onDevice.IsAvailable)
            {
                var move = await GetOnDeviceMoveAsync(board);
                if (move >= 0) return move;
                // On-device failed, will try cloud next
            }
        }
        catch { }
#endif

        // Fall back to cloud AI
        if (!string.IsNullOrEmpty(ApiKey))
        {
            var move = await GetCloudMoveAsync(board);
            if (move >= 0) return move;
        }

        LastDebugInfo = "⚙️ Built-in strategy";
        return -1;
    }

    private static async Task<int> GetCloudMoveAsync(string[] board)
    {
        try
        {
            var boardStr = FormatBoard(board);
            var prompt = $"TicTacToe as O. Board:\n{boardStr}\nReply with ONE digit (0-8) for empty (.) position.";

            var request = new
            {
                model = Model,
                messages = new[] { new { role = "user", content = prompt } },
                max_tokens = 5,
                temperature = 0.1
            };

            var json = JsonSerializer.Serialize(request);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, ApiEndpoint)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
            httpRequest.Headers.Add("Authorization", $"Bearer {ApiKey}");
            
            var response = await _httpClient.Value.SendAsync(httpRequest);
            if (!response.IsSuccessStatusCode)
            {
                LastDebugInfo = $"☁️ {Model} · ❌ {response.StatusCode}";
                return -1;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(responseBody);
            
            var content = result
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()?.Trim() ?? "";

            // Extract first digit from response
            foreach (char c in content)
            {
                if (char.IsDigit(c))
                {
                    int move = c - '0';
                    if (move >= 0 && move <= 8 && string.IsNullOrEmpty(board[move]))
                    {
                        LastDebugInfo = $"☁️ {Model} · ✓ \"{content}\"";
                        return move;
                    }
                }
            }
            LastDebugInfo = $"☁️ {Model} · ⚠️ \"{content}\" invalid";
        }
        catch (Exception ex)
        {
            LastDebugInfo = $"☁️ Cloud AI · ❌ {ex.Message}";
        }
        
        return -1;
    }

    private static async Task<int> GetOnDeviceMoveAsync(string[] board)
    {
        try
        {
            var boardStr = FormatBoard(board);
            var prompt = $"TicTacToe: You are O. Board:\n{boardStr}\nReply with ONE digit (0-8) for empty (.) position.";

            var generator = new OnDeviceAIExcuseGenerator();
            if (!generator.IsAvailable)
            {
                return -1;
            }

            var result = await generator.GenerateExcuseAsync(prompt);
            var content = result.Excuse;

            // Extract first valid digit
            foreach (char c in content)
            {
                if (char.IsDigit(c))
                {
                    int move = c - '0';
                    if (move >= 0 && move <= 8 && string.IsNullOrEmpty(board[move]))
                    {
                        LastDebugInfo = $"🍎 Apple Intelligence · ✓ \"{content}\"";
                        return move;
                    }
                }
            }
            LastDebugInfo = $"🍎 Apple Intelligence · ⚠️ \"{content}\" invalid";
        }
        catch (Exception ex)
        {
            LastDebugInfo = $"🍎 Apple Intelligence · ❌ {ex.Message}";
        }
        
        return -1;
    }

    private static string FormatBoard(string[] board)
    {
        string Cell(int i) => string.IsNullOrEmpty(board[i]) ? "." : board[i];
        return $"{Cell(0)}|{Cell(1)}|{Cell(2)}\n{Cell(3)}|{Cell(4)}|{Cell(5)}\n{Cell(6)}|{Cell(7)}|{Cell(8)}";
    }
}
