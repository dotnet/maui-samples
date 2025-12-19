using procrastinate.Services;

namespace procrastinate.Pages;

public partial class ExcusePage : ContentPage
{
    private readonly StatsService _statsService;
    private readonly ExcuseService _excuseService;
    private string _currentExcuse = "";

    public ExcusePage(StatsService statsService, ExcuseService excuseService)
    {
        InitializeComponent();
        _statsService = statsService;
        _excuseService = excuseService;
        UpdateCounterLabel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateCounterLabel();
    }

    private void UpdateCounterLabel()
    {
        CounterLabel.Text = AppStrings.GetString("ExcusesGenerated", _statsService.ExcusesGenerated);
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    private async void OnGenerateClicked(object? sender, EventArgs e)
    {
        // Refresh zalgo randomness on button click
        AppStrings.Refresh();
        
        ShareIconBtn.IsVisible = false;
        GeneratorInfoLabel.IsVisible = false;
        
        // Show loading state
        GenerateBtn.IsEnabled = false;
        ExcuseLabel.Text = AppStrings.GetString("Generating");

        try
        {
            var result = await _excuseService.GenerateExcuseAsync(AppStrings.EffectiveLanguage);
            _currentExcuse = result.Excuse;
            
            // Never apply Zalgo to the excuse itself
            ExcuseLabel.Text = _currentExcuse;
            
            _statsService.IncrementExcusesGenerated();
            UpdateCounterLabel();
            
            // Show generator info
            UpdateGeneratorInfo(result);
            
            // Show the share button
            ShareIconBtn.IsVisible = true;
        }
        catch (Exception ex)
        {
            ExcuseLabel.Text = $"Error: {ex.Message}";
        }
        finally
        {
            GenerateBtn.IsEnabled = true;
        }
    }

    private void UpdateGeneratorInfo(ExcuseResult result)
    {
        var parts = new List<string> { result.GeneratorName };
        
        if (result.Model != null)
        {
            parts.Add(result.Model);
        }
        
        parts.Add($"{result.Duration.TotalMilliseconds:F0}ms");
        
        if (result.TokenCount.HasValue)
        {
            parts.Add($"{result.TokenCount} tokens");
        }
        
        GeneratorInfoLabel.Text = string.Join(" · ", parts);
        GeneratorInfoLabel.IsVisible = true;
    }

    private async void OnShareClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_currentExcuse)) return;
        
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Text = _currentExcuse,
            Title = AppStrings.GetString("ShareExcuse")
        });
    }
}
