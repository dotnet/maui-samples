using System.Text.Json;

namespace procrastinate.Services;

public class StatsService
{
    private const string StatsKey = "DailyStats";
    private const string HighScoresKey = "GameHighScores";
    
    private Dictionary<string, DailyStat> _dailyStats = [];
    private Dictionary<string, int> _gameHighScores = [];

    public static StatsService? Instance { get; private set; }

    public StatsService()
    {
        Instance = this;
        LoadStats();
    }

    public int TasksAvoided => _dailyStats.Values.Sum(s => s.TasksAvoided);
    public int ExcusesGenerated => _dailyStats.Values.Sum(s => s.ExcusesGenerated);
    public int GamesPlayed => _dailyStats.Values.Sum(s => s.GamesPlayed);
    public int BreaksTaken => _dailyStats.Values.Sum(s => s.BreaksTaken);
    public int AIExcuseCalls => _dailyStats.Values.Sum(s => s.AIExcuseCalls);
    public int TotalClicks => _dailyStats.Values.Sum(s => s.TotalClicks);
    public IReadOnlyDictionary<string, int> GameHighScores => _gameHighScores;

    // Today's stats
    public DailyStat TodayStats => GetOrCreateToday();

    // Get stats for last N days
    public List<(DateTime Date, DailyStat Stats)> GetDailyStats(int days)
    {
        var result = new List<(DateTime, DailyStat)>();
        var today = DateTime.Today;
        
        for (int i = days - 1; i >= 0; i--)
        {
            var date = today.AddDays(-i);
            var key = date.ToString("yyyy-MM-dd");
            var stat = _dailyStats.TryGetValue(key, out var s) ? s : new DailyStat();
            result.Add((date, stat));
        }
        return result;
    }

    // Get weekly totals (last 4 weeks)
    public List<(int WeekNumber, DailyStat Stats)> GetWeeklyStats()
    {
        var result = new List<(int, DailyStat)>();
        var today = DateTime.Today;
        
        for (int w = 3; w >= 0; w--)
        {
            var weekStart = today.AddDays(-((int)today.DayOfWeek) - (w * 7));
            var weekStat = new DailyStat();
            
            for (int d = 0; d < 7; d++)
            {
                var date = weekStart.AddDays(d);
                var key = date.ToString("yyyy-MM-dd");
                if (_dailyStats.TryGetValue(key, out var s))
                {
                    weekStat.TasksAvoided += s.TasksAvoided;
                    weekStat.ExcusesGenerated += s.ExcusesGenerated;
                    weekStat.GamesPlayed += s.GamesPlayed;
                    weekStat.BreaksTaken += s.BreaksTaken;
                    weekStat.AIExcuseCalls += s.AIExcuseCalls;
                }
            }
            
            var weekNum = System.Globalization.ISOWeek.GetWeekOfYear(weekStart);
            result.Add((weekNum, weekStat));
        }
        return result;
    }

    private DailyStat GetOrCreateToday()
    {
        var key = DateTime.Today.ToString("yyyy-MM-dd");
        if (!_dailyStats.TryGetValue(key, out var stat))
        {
            stat = new DailyStat();
            _dailyStats[key] = stat;
        }
        return stat;
    }

    public void IncrementTasksAvoided()
    {
        GetOrCreateToday().TasksAvoided++;
        SaveStats();
    }

    public void IncrementExcusesGenerated()
    {
        GetOrCreateToday().ExcusesGenerated++;
        SaveStats();
    }

    public void IncrementGamesPlayed()
    {
        GetOrCreateToday().GamesPlayed++;
        SaveStats();
    }

    public void IncrementBreaksTaken()
    {
        GetOrCreateToday().BreaksTaken++;
        SaveStats();
    }

    public void IncrementAIExcuseCalls()
    {
        GetOrCreateToday().AIExcuseCalls++;
        SaveStats();
    }

    public void IncrementClicks()
    {
        GetOrCreateToday().TotalClicks++;
        SaveStats();
    }

    public void UpdateHighScore(string gameName, int score)
    {
        if (!_gameHighScores.TryGetValue(gameName, out var current) || score > current)
        {
            _gameHighScores[gameName] = score;
            SaveStats();
        }
    }

    public int GetHighScore(string gameName) => _gameHighScores.TryGetValue(gameName, out var score) ? score : 0;

    private void LoadStats()
    {
        try
        {
            var statsJson = Preferences.Get(StatsKey, "{}");
            _dailyStats = JsonSerializer.Deserialize<Dictionary<string, DailyStat>>(statsJson) ?? [];
            
            var scoresJson = Preferences.Get(HighScoresKey, "{}");
            _gameHighScores = JsonSerializer.Deserialize<Dictionary<string, int>>(scoresJson) ?? [];
        }
        catch
        {
            _dailyStats = [];
            _gameHighScores = [];
        }
    }

    private void SaveStats()
    {
        try
        {
            Preferences.Set(StatsKey, JsonSerializer.Serialize(_dailyStats));
            Preferences.Set(HighScoresKey, JsonSerializer.Serialize(_gameHighScores));
        }
        catch { }
    }
}

public class DailyStat
{
    public int TasksAvoided { get; set; }
    public int ExcusesGenerated { get; set; }
    public int GamesPlayed { get; set; }
    public int BreaksTaken { get; set; }
    public int AIExcuseCalls { get; set; }
    public int TotalClicks { get; set; }
    
    public int Total => TasksAvoided + ExcusesGenerated + GamesPlayed + BreaksTaken;
}
