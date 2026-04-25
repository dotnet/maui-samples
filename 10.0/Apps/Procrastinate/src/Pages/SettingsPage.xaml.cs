using procrastinate.Services;

namespace procrastinate.Pages;

public partial class SettingsPage : ContentPage
{
    private static readonly string[] GroqModels = 
    {
        "llama-3.3-70b-versatile",
        "llama-3.1-8b-instant",
        "gemma2-9b-it"
    };

    private readonly ExcuseService _excuseService;

    public SettingsPage(ExcuseService excuseService)
    {
        _excuseService = excuseService;
        InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()
    {
        // Load language picker
        foreach (var lang in AppStrings.SupportedLanguages)
            LanguagePicker.Items.Add(lang.Value);
        
        var savedLang = Preferences.Get("AppLanguage", "");
        var langIndex = AppStrings.SupportedLanguages.Keys.ToList().IndexOf(savedLang);
        LanguagePicker.SelectedIndex = langIndex >= 0 ? langIndex : 0;

        // Load high contrast
        var isHighContrast = Preferences.Get("HighContrastMode", false);
        HighContrastSwitch.IsToggled = isHighContrast;
        UpdatePreview(isHighContrast);
        UpdateThemeLabel(isHighContrast);

        // Load Zalgo mode - always show zalgo text for the description
        ZalgoSwitch.IsToggled = AppStrings.IsZalgoMode;
        var zalgoDesc = AppStrings.Instance["ZalgoModeDesc"];
        ZalgoDescLabel.Text = AppStrings.Zalgoify(zalgoDesc);

        // Load excuse engine settings
        foreach (var mode in ExcuseService.AvailableModes)
            ExcuseModePicker.Items.Add(mode.Value);
        
        var currentMode = ExcuseService.CurrentMode;
        var modeIndex = ExcuseService.AvailableModes.Keys.ToList().IndexOf(currentMode);
        ExcuseModePicker.SelectedIndex = modeIndex >= 0 ? modeIndex : 0;

        GroqApiKeyEntry.Text = SecureStorage.GetAsync("GroqApiKey").GetAwaiter().GetResult() ?? "";
        
        // Load Groq models picker
        foreach (var model in GroqModels)
            GroqModelPicker.Items.Add(model);
        
        var savedModel = Preferences.Get("GroqModel", "llama-3.3-70b-versatile");
        var modelIndex = Array.IndexOf(GroqModels, savedModel);
        GroqModelPicker.SelectedIndex = modelIndex >= 0 ? modelIndex : 0;
        
        UpdateAISettingsVisibility();

        // Display version
        var version = AppInfo.VersionString;
        var build = AppInfo.BuildString;
        VersionLabel.Text = $"Procrastinate v{version} (build {build})";
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        if (LanguagePicker.SelectedIndex < 0) return;
        
        var langCode = AppStrings.SupportedLanguages.Keys.ElementAt(LanguagePicker.SelectedIndex);
        AppStrings.CurrentLanguage = langCode;
        UpdateThemeLabel(Preferences.Get("HighContrastMode", false));
    }

    private void UpdateThemeLabel(bool isHighContrast)
    {
        var themeName = isHighContrast ? AppStrings.GetString("HighContrast") : AppStrings.GetString("DefaultTheme");
        ThemeLabel.Text = AppStrings.GetString("CurrentTheme", themeName);
    }

    private void OnHighContrastToggled(object? sender, ToggledEventArgs e)
    {
        Preferences.Set("HighContrastMode", e.Value);
        UpdatePreview(e.Value);
        // Use AppTheme to switch - AppThemeBinding in XAML handles the rest
        if (Application.Current != null)
            Application.Current.UserAppTheme = e.Value ? AppTheme.Light : AppTheme.Dark;
        UpdateThemeLabel(e.Value);
    }

    private void OnZalgoToggled(object? sender, ToggledEventArgs e)
    {
        AppStrings.IsZalgoMode = e.Value;
    }

    private void OnExcuseModeChanged(object? sender, EventArgs e)
    {
        if (ExcuseModePicker.SelectedIndex < 0) return;
        
        var modeKey = ExcuseService.AvailableModes.Keys.ElementAt(ExcuseModePicker.SelectedIndex);
        ExcuseService.CurrentMode = modeKey;
        UpdateAISettingsVisibility();
    }

    private void UpdateAISettingsVisibility()
    {
        CloudSettingsPanel.IsVisible = ExcuseService.CurrentMode == "cloud";
        OnDeviceAISettingsPanel.IsVisible = ExcuseService.CurrentMode == "ondevice";
        PipelineSettingsPanel.IsVisible = ExcuseService.CurrentMode == "pipeline";
        CustomEndpointPanel.IsVisible = ExcuseService.CurrentMode == "custom";
        EmbeddedModelPanel.IsVisible = ExcuseService.CurrentMode == "embedded";
        
        if (ExcuseService.CurrentMode == "ondevice")
        {
            UpdateOnDeviceAIStatus();
        }
        if (ExcuseService.CurrentMode == "custom")
        {
            LoadCustomEndpointSettings();
        }
        if (ExcuseService.CurrentMode == "embedded")
        {
            UpdateEmbeddedModelStatus();
        }
    }

    private void UpdateOnDeviceAIStatus()
    {
        if (_excuseService.IsOnDeviceAvailable)
        {
            OnDeviceAIStatusLabel.Text = "✅ " + AppStrings.Instance.OnDeviceAIAvailable + " (via MEAI IChatClient)";
            OnDeviceAIStatusLabel.TextColor = Color.FromArgb("#88C0D0");
        }
        else
        {
            OnDeviceAIStatusLabel.Text = AppStrings.Instance.OnDeviceAIUnavailable;
            OnDeviceAIStatusLabel.TextColor = Color.FromArgb("#D08770");
        }
    }

    private void OnGroqApiKeyChanged(object? sender, TextChangedEventArgs e)
    {
        _ = SecureStorage.SetAsync("GroqApiKey", e.NewTextValue ?? "");
    }

    private void OnGroqModelChanged(object? sender, EventArgs e)
    {
        if (GroqModelPicker.SelectedIndex < 0) return;
        var model = GroqModels[GroqModelPicker.SelectedIndex];
        Preferences.Set("GroqModel", model);
    }

    private void UpdatePreview(bool highContrast)
    {
        if (highContrast)
        {
            // Nord Light preview
            PreviewPrimary.BackgroundColor = Color.FromArgb("#5E81AC");
            PreviewSecondary.BackgroundColor = Color.FromArgb("#81A1C1");
            PreviewTertiary.BackgroundColor = Color.FromArgb("#88C0D0");
            PreviewAccent.BackgroundColor = Color.FromArgb("#A3BE8C");
        }
        else
        {
            // Nord Dark preview
            PreviewPrimary.BackgroundColor = Color.FromArgb("#88C0D0");
            PreviewSecondary.BackgroundColor = Color.FromArgb("#81A1C1");
            PreviewTertiary.BackgroundColor = Color.FromArgb("#5E81AC");
            PreviewAccent.BackgroundColor = Color.FromArgb("#A3BE8C");
        }
    }

    private async void OnGitHubTapped(object? sender, EventArgs e)
    {
        try
        {
            await Launcher.OpenAsync("https://github.com/StephaneDelcroix/procrastinate");
        }
        catch { }
    }

    private void LoadCustomEndpointSettings()
    {
        CustomEndpointEntry.Text = Preferences.Get("CustomAIEndpoint", "");
        CustomApiKeyEntry.Text = SecureStorage.GetAsync("CustomAIApiKey").GetAwaiter().GetResult() ?? "";
        CustomModelEntry.Text = Preferences.Get("CustomAIModel", "");
    }

    private void OnCustomEndpointChanged(object? sender, TextChangedEventArgs e)
    {
        Preferences.Set("CustomAIEndpoint", e.NewTextValue ?? "");
    }

    private void OnCustomApiKeyChanged(object? sender, TextChangedEventArgs e)
    {
        _ = SecureStorage.SetAsync("CustomAIApiKey", e.NewTextValue ?? "");
    }

    private void OnCustomModelChanged(object? sender, TextChangedEventArgs e)
    {
        Preferences.Set("CustomAIModel", e.NewTextValue ?? "");
    }

    // -- Embedded ONNX Model --

    private CancellationTokenSource? _downloadCts;

    private void UpdateEmbeddedModelStatus()
    {
        var model = OnnxModelManager.AvailableModels[0];
        if (OnnxModelManager.IsModelDownloaded(model.Id))
        {
            var size = OnnxModelManager.GetDownloadedSize(model.Id);
            EmbeddedModelStatusLabel.Text = $"✅ {model.Name} — Ready ({size / (1024.0 * 1024 * 1024):F1} GB)";
            EmbeddedModelStatusLabel.TextColor = Color.FromArgb("#A3BE8C");
            EmbeddedDownloadBtn.IsVisible = false;
            EmbeddedDeleteBtn.IsVisible = true;
            EmbeddedDownloadProgress.IsVisible = false;
            EmbeddedDownloadDetailLabel.IsVisible = false;
        }
        else
        {
            EmbeddedModelStatusLabel.Text = $"⬇ {model.Name} — Not downloaded";
            EmbeddedModelStatusLabel.TextColor = Color.FromArgb("#D08770");
            EmbeddedDownloadBtn.IsVisible = true;
            EmbeddedDownloadBtn.Text = $"Download {model.Name}";
            EmbeddedDeleteBtn.IsVisible = false;
        }
    }

    private async void OnDownloadEmbeddedModel(object? sender, EventArgs e)
    {
        var model = OnnxModelManager.AvailableModels[0];
        _downloadCts?.Cancel();
        _downloadCts = new CancellationTokenSource();

        EmbeddedDownloadBtn.IsEnabled = false;
        EmbeddedDownloadBtn.Text = "Downloading...";
        EmbeddedDownloadProgress.IsVisible = true;
        EmbeddedDownloadProgress.Progress = 0;
        EmbeddedDownloadDetailLabel.IsVisible = true;

        var progress = new Progress<(long downloaded, long total, string file)>(p =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var pct = p.total > 0 ? (double)p.downloaded / p.total : 0;
                EmbeddedDownloadProgress.Progress = pct;
                var dlMB = p.downloaded / (1024.0 * 1024);
                var totalMB = p.total / (1024.0 * 1024);
                EmbeddedDownloadDetailLabel.Text = $"{dlMB:F0} / {totalMB:F0} MB ({pct:P0}) — {p.file}";
            });
        });

        try
        {
            await OnnxModelManager.DownloadModelAsync(model, progress, _downloadCts.Token);
            UpdateEmbeddedModelStatus();
        }
        catch (OperationCanceledException)
        {
            EmbeddedModelStatusLabel.Text = "Download cancelled.";
        }
        catch (Exception ex)
        {
            EmbeddedModelStatusLabel.Text = $"❌ Download failed: {ex.Message}";
            EmbeddedDownloadBtn.IsEnabled = true;
            EmbeddedDownloadBtn.Text = "Retry Download";
        }
    }

    private async void OnDeleteEmbeddedModel(object? sender, EventArgs e)
    {
        var confirm = await DisplayAlert("Delete Model",
            "Delete the downloaded ONNX model? This frees ~2.5 GB of storage.",
            "Delete", "Cancel");
        if (!confirm) return;

        var model = OnnxModelManager.AvailableModels[0];
#if !IOS
        OnnxGenAIChatClient.UnloadCached();
#endif
        OnnxModelManager.DeleteModel(model.Id);
        UpdateEmbeddedModelStatus();
    }
}
