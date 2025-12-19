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

    public SettingsPage()
    {
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

        GroqApiKeyEntry.Text = Preferences.Get("GroqApiKey", "");
        
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
        
        if (ExcuseService.CurrentMode == "ondevice")
        {
            UpdateOnDeviceAIStatus();
        }
    }

    private void UpdateOnDeviceAIStatus()
    {
        var generator = new OnDeviceAIExcuseGenerator();
        if (generator.IsAvailable)
        {
            OnDeviceAIStatusLabel.Text = AppStrings.Instance.OnDeviceAIAvailable;
            OnDeviceAIStatusLabel.TextColor = Color.FromArgb("#88C0D0"); // Secondary/teal
        }
        else
        {
            OnDeviceAIStatusLabel.Text = AppStrings.Instance.OnDeviceAIUnavailable;
            OnDeviceAIStatusLabel.TextColor = Color.FromArgb("#D08770"); // Primary/amber warning
        }
    }

    private void OnGroqApiKeyChanged(object? sender, TextChangedEventArgs e)
    {
        Preferences.Set("GroqApiKey", e.NewTextValue ?? "");
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
}
