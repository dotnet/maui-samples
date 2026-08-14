using Microsoft.Maui.Controls.Shapes;
using procrastinate.Services;

namespace procrastinate.Pages;

public partial class StatsPage : ContentPage
{
    private readonly StatsService _statsService;

    public StatsPage(StatsService statsService)
    {
        InitializeComponent();
        _statsService = statsService;
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshStats();
        RefreshChart();
    }

    private void RefreshStats()
    {
        TasksAvoidedLabel.Text = _statsService.TasksAvoided.ToString();
        BreaksTakenLabel.Text = _statsService.BreaksTaken.ToString();
        ExcusesLabel.Text = _statsService.ExcusesGenerated.ToString();
        GamesPlayedLabel.Text = _statsService.GamesPlayed.ToString();
        AICallsLabel.Text = _statsService.AIExcuseCalls.ToString();
        TotalClicksLabel.Text = _statsService.TotalClicks.ToString();

        RefreshHighScores();

        var totalActivity = _statsService.TasksAvoided + _statsService.BreaksTaken + 
                           _statsService.ExcusesGenerated + _statsService.GamesPlayed;
        
        AchievementLabel.Text = totalActivity switch
        {
            0 => $"{AppStrings.GetString("GettingStarted")} ✅",
            < 5 => $"{AppStrings.GetString("BeginnerProcrastinator")} 🐣",
            < 15 => GetRandomAchievement(),
            _ => $"🌟 {AppStrings.GetString("LegendaryProcrastinator")} 🌟"
        };
    }

    private void RefreshChart()
    {
        ChartTitleLabel.Text = AppStrings.GetString("Last7Days");
        ChartGrid.Children.Clear();

        var dailyStats = _statsService.GetDailyStats(7);
        var maxTotal = dailyStats.Max(d => d.Stats.Total);
        if (maxTotal == 0) maxTotal = 1;

        for (int i = 0; i < dailyStats.Count; i++)
        {
            var (date, stats) = dailyStats[i];
            var heightPercent = (double)stats.Total / maxTotal;
            
            var barContainer = new Grid
            {
                RowDefinitions = [new RowDefinition(GridLength.Star), new RowDefinition(GridLength.Auto)]
            };

            var bar = new Border
            {
                BackgroundColor = GetBarColor(i),
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(4, 4, 0, 0) },
                Stroke = Colors.Transparent,
                HeightRequest = Math.Max(4, 80 * heightPercent),
                VerticalOptions = LayoutOptions.End
            };

            var dayLabel = new Label
            {
                Text = date.ToString("ddd")[..2],
                FontSize = 10,
                TextColor = (Color)Application.Current!.Resources["Nord4"],
                HorizontalOptions = LayoutOptions.Center
            };

            var countLabel = new Label
            {
                Text = stats.Total > 0 ? stats.Total.ToString() : "",
                FontSize = 9,
                TextColor = (Color)Application.Current!.Resources["Nord4"],
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                Margin = new Thickness(0, 0, 0, 4)
            };

            barContainer.Add(bar, 0, 0);
            barContainer.Add(countLabel, 0, 0);
            barContainer.Add(dayLabel, 0, 1);
            
            ChartGrid.Add(barContainer, i, 0);
        }
    }

    private static Color GetBarColor(int index)
    {
        var colors = new[]
        {
            Color.FromArgb("#88C0D0"),
            Color.FromArgb("#D08770"),
            Color.FromArgb("#B48EAD"),
            Color.FromArgb("#B48EAD"),
            Color.FromArgb("#81A1C1"),
            Color.FromArgb("#A3BE8C"),
            Color.FromArgb("#D08770")
        };
        return colors[index % colors.Length];
    }

    private void RefreshHighScores()
    {
        var highScores = _statsService.GameHighScores;
        
        if (highScores.Count == 0)
        {
            NoHighScoresLabel.IsVisible = true;
            return;
        }

        NoHighScoresLabel.IsVisible = false;
        
        var toRemove = HighScoresStack.Children.Where(c => c != NoHighScoresLabel).ToList();
        foreach (var child in toRemove)
            HighScoresStack.Children.Remove(child);

        foreach (var (game, score) in highScores.OrderByDescending(x => x.Value))
        {
            var scoreLabel = new Label
            {
                Text = $"{game}: {score}",
                FontSize = 16,
                TextColor = (Color)Application.Current!.Resources["Nord5"]
            };
            HighScoresStack.Children.Add(scoreLabel);
        }
    }

    private string GetRandomAchievement()
    {
        var achievements = AppStrings.CurrentLanguage switch
        {
            "fr" => new[] { "Professionnel de la pause 🛋️", "Maître de demain 📅", "Artiste des excuses 🎨", "Rebelle de la productivité 😎" },
            "es" => new[] { "Profesional del descanso 🛋️", "Maestro del mañana 📅", "Artista de excusas 🎨", "Rebelde de productividad 😎" },
            "pt" => new[] { "Profissional da pausa 🛋️", "Mestre do amanhã 📅", "Artista de desculpas 🎨", "Rebelde da produtividade 😎" },
            "nl" => new[] { "Professionele pauzenemer 🛋️", "Meester van morgen 📅", "Excuuskunstenaar 🎨", "Productiviteitsrebel 😎" },
            "cs" => new[] { "Profesionální pausař 🛋️", "Mistr zítřka 📅", "Umělec výmluv 🎨", "Rebel produktivity 😎" },
            _ => new[] { "Professional Break Taker 🛋️", "Master of Tomorrow 📅", "Expert Excuse Artist 🎨", "Productivity Rebel 😎" }
        };
        return achievements[Random.Shared.Next(achievements.Length)];
    }
}
