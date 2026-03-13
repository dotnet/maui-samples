using System.ComponentModel;
using Microsoft.Extensions.AI;

namespace LocalChatClientWithTools.Services.Tools;

public class TimerTool
{
    public static AIFunction CreateAIFunction(IServiceProvider services)
        => AIFunctionFactory.Create(
            services.GetRequiredService<TimerTool>().SetTimer,
            name: "set_timer",
            serializerOptions: ToolJsonContext.Default.Options);

    private static readonly Dictionary<string, System.Timers.Timer> _activeTimers = [];

    [Description("Sets a timer for a specified number of minutes with a title")]
    public TimerResult SetTimer(
        [Description("Duration in minutes (1-1440)")] int minutes,
        [Description("Title for the timer")] string title = "Timer")
    {
        if (string.IsNullOrEmpty(title))
            title = minutes > 0 ? $"{minutes}-minute timer" : "Timer";

        if (minutes <= 0 || minutes > 1440)
        {
            return new TimerResult
            {
                DurationMinutes = Math.Clamp(minutes, 0, 1440),
                Title = "Invalid timer request",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now,
                TimerId = string.Empty
            };
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

        return new TimerResult
        {
            DurationMinutes = minutes,
            Title = title,
            StartTime = start,
            EndTime = end,
            TimerId = timerId
        };
    }

    private static void OnTimerElapsed(string timerId, string title)
    {
        if (_activeTimers.TryGetValue(timerId, out var timer))
        {
            timer.Dispose();
            _activeTimers.Remove(timerId);
        }

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (Application.Current?.Windows.FirstOrDefault()?.Page is Page page)
                await page.DisplayAlertAsync("⏰ Timer Finished", $"'{title}' is done!", "OK");
        });
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

    public record TimerResult
    {
        public required int DurationMinutes { get; init; }
        public required string Title { get; init; }
        public required DateTime StartTime { get; init; }
        public required DateTime EndTime { get; init; }
        public required string TimerId { get; init; }
    }
}
