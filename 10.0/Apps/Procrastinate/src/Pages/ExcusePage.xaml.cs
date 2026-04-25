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

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _excuseService.OnPipelineStageChanged = null;
        _excuseService.OnAgentOutput = null;
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
        PipelineStageLabel.IsVisible = false;
        AgentReasoningBorder.IsVisible = false;
        AgentReasoningContent.IsVisible = false;
        AgentOutputStack.Children.Clear();
        
        // Show loading state
        GenerateBtn.IsEnabled = false;
        ExcuseLabel.Text = AppStrings.GetString("Generating");

        // Wire up pipeline stage callback for agent pipeline mode
        _excuseService.OnPipelineStageChanged = (stage) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                PipelineStageLabel.Text = stage;
                PipelineStageLabel.IsVisible = true;
            });
        };

        // Wire up agent reasoning output
        _excuseService.OnAgentOutput = (agentName, output) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var card = new VerticalStackLayout { Spacing = 4 };
                card.Children.Add(new Label
                {
                    Text = agentName,
                    FontSize = 13,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#EBCB8B") // Nord13 — yellow, high contrast on both themes
                });
                card.Children.Add(new Label
                {
                    Text = output,
                    FontSize = 13,
                    TextColor = Application.Current?.RequestedTheme == AppTheme.Light
                        ? Color.FromArgb("#2E3440")   // Nord0 — dark text on light bg
                        : Color.FromArgb("#ECEFF4"),   // Nord6 — bright text on dark bg
                    LineBreakMode = LineBreakMode.WordWrap
                });
                AgentOutputStack.Children.Add(card);
            });
        };

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
            
            // Show reasoning panel if pipeline mode produced agent outputs
            if (AgentOutputStack.Children.Count > 0)
                AgentReasoningBorder.IsVisible = true;
            
            // Show the share button
            ShareIconBtn.IsVisible = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Excuse generation error: {ex}");
            ExcuseLabel.Text = "Something went wrong. Tap to try again! 🔄";
        }
        finally
        {
            GenerateBtn.IsEnabled = true;
        }
    }

    private void UpdateGeneratorInfo(ExcuseResult result)
    {
        var parts = new List<string>();

        // Show MEAI badge for AI-powered generators
        if (result.GeneratorName.Contains("AI"))
            parts.Add("MEAI");

        parts.Add(result.GeneratorName);

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

    private void OnToggleReasoning(object? sender, EventArgs e)
    {
        AgentReasoningContent.IsVisible = !AgentReasoningContent.IsVisible;
        ReasoningToggleIcon.Text = AgentReasoningContent.IsVisible ? "\uf077" : "\uf078"; // chevron-up / chevron-down
    }
}
