using Microsoft.Maui.Controls;

namespace procrastinate.Pages.Games;

public abstract class MiniGame
{
    public abstract string Name { get; }
    public abstract string Icon { get; }
    public abstract string IconColor { get; }
    public abstract string Description { get; }
    public abstract View CreateGameView();
    public abstract void StartGame();
    public abstract void StopGame();
    
    public Action? OnGamePlayed { get; set; }
    public Action<string, int>? OnHighScore { get; set; }
    public Action<MiniGame>? OnFavoriteToggled { get; set; }
    
    public string Id => GetType().Name;
    
    public bool IsFavorite
    {
        get => Preferences.Get($"favorite_{Id}", false);
        set => Preferences.Set($"favorite_{Id}", value);
    }
}
