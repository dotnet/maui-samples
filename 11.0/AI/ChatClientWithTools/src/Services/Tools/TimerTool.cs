using Microsoft.Extensions.AI;
using System.Text.Json;
using ChatClientWithTools.Models;
using System.Linq;

namespace ChatClientWithTools.Services.Tools;

public class TimerTool : AIFunction
{
    private static readonly Dictionary<string, System.Timers.Timer> _activeTimers = new();

    public override string Name => "set_timer";
    public override string Description => "Sets a timer for a specified number of minutes with an optional title";

    public override JsonElement JsonSchema => JsonSerializer.SerializeToElement(new
    {
        type = "object",
        properties = new
        {
            minutes = new { type = "integer", description = "Duration in minutes (1-1440)", minimum = 1, maximum = 1440 },
            title = new { type = "string", description = "Optional title for the timer" }
        },
        required = new[] { "minutes" }
    });

    protected override ValueTask<object?> InvokeCoreAsync(AIFunctionArguments arguments, CancellationToken cancellationToken)
    {
        var minutes = GetInt(arguments, "minutes", 0);
        var title = GetString(arguments, "title", minutes > 0 ? $"{minutes}-minute timer" : "Timer");

        if (minutes <= 0 || minutes > 1440)
        {
            return ValueTask.FromResult<object?>(new TimerResult
            {
                DurationMinutes = Math.Clamp(minutes, 0, 1440),
                Title = "Invalid timer request",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now,
                TimerId = string.Empty
            });
        }

        var timerId = Guid.NewGuid().ToString();
        var start = DateTime.Now;
        var end = start.AddMinutes(minutes);

        var timer = new System.Timers.Timer(minutes * 60 * 1000)
        {
            AutoReset = false
        };
        timer.Elapsed += (_, _) => OnTimerElapsed(timerId, title);
        timer.Start();
        _activeTimers[timerId] = timer;

        return ValueTask.FromResult<object?>(new TimerResult
        {
            DurationMinutes = minutes,
            Title = title,
            StartTime = start,
            EndTime = end,
            TimerId = timerId
        });
    }

    private static string GetString(AIFunctionArguments args, string name, string def = "") =>
        args.TryGetValue(name, out var v) ? v?.ToString() ?? def : def;

    private static int GetInt(AIFunctionArguments args, string name, int def = 0) =>
        args.TryGetValue(name, out var v) && int.TryParse(v?.ToString(), out var i) ? i : def;

    private static void OnTimerElapsed(string timerId, string title)
    {
        if (_activeTimers.TryGetValue(timerId, out var timer))
        {
            timer.Dispose();
            _activeTimers.Remove(timerId);
        }
        Console.WriteLine($"🔔 Timer '{title}' finished ({timerId}).");
    }

    public static bool CancelTimer(string timerId)
    {
        if (_activeTimers.TryGetValue(timerId, out var timer))
        {
            timer.Stop();
            timer.Dispose();
            _activeTimers.Remove(timerId);
            return true;
        }
        return false;
    }

    public static IReadOnlyDictionary<string, System.Timers.Timer> GetActiveTimers() => _activeTimers;
}