using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace procrastinate.Services;

public class AppStrings : INotifyPropertyChanged
{
    private static readonly Lazy<AppStrings> _instance = new(() => new AppStrings());
    public static AppStrings Instance => _instance.Value;

    private static readonly ResourceManager _resourceManager = 
        new("procrastinate.Resources.Strings.AppResources", typeof(AppStrings).Assembly);

    private CultureInfo _culture;
    private bool _zalgoMode;

    // Zalgo combining characters
    private static readonly char[] ZalgoUp = {
        '\u0300', '\u0301', '\u0302', '\u0303', '\u0304', '\u0305', '\u0306', '\u0307',
        '\u0308', '\u0309', '\u030a', '\u030b', '\u030c', '\u030d', '\u030e', '\u030f',
        '\u0310', '\u0311', '\u0312', '\u0313', '\u0314', '\u0315', '\u031a', '\u031b',
        '\u033d', '\u033e', '\u033f', '\u0340', '\u0341', '\u0342', '\u0343', '\u0344'
    };
    private static readonly char[] ZalgoDown = {
        '\u0316', '\u0317', '\u0318', '\u0319', '\u031c', '\u031d', '\u031e', '\u031f',
        '\u0320', '\u0321', '\u0322', '\u0323', '\u0324', '\u0325', '\u0326', '\u0327',
        '\u0328', '\u0329', '\u032a', '\u032b', '\u032c', '\u032d', '\u032e', '\u032f',
        '\u0330', '\u0331', '\u0332', '\u0333', '\u0339', '\u033a', '\u033b', '\u033c'
    };
    private static readonly char[] ZalgoMid = { '\u0334', '\u0335', '\u0336', '\u0337', '\u0338' };
    private static readonly Random _random = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public static readonly Dictionary<string, string> SupportedLanguages = new()
    {
        { "", "System Default" },
        { "en", "English" },
        { "fr", "Français" },
        { "es", "Español" },
        { "pt", "Português" },
        { "nl", "Nederlands" },
        { "cs", "Čeština" },
        { "uk", "Українська" }
    };

    private AppStrings()
    {
        var savedLang = Preferences.Get("AppLanguage", "");
        if (string.IsNullOrEmpty(savedLang))
        {
            // Use system language, fall back to English if not supported
            var systemLang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            savedLang = SupportedLanguages.ContainsKey(systemLang) ? systemLang : "en";
        }
        _culture = new CultureInfo(savedLang);
        _zalgoMode = Preferences.Get("ZalgoMode", true); // Default ON
    }

    public static string CurrentLanguage
    {
        get => Preferences.Get("AppLanguage", "");
        set
        {
            Preferences.Set("AppLanguage", value);
            if (string.IsNullOrEmpty(value))
            {
                // Use system language, fall back to English if not supported
                var systemLang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                var effectiveLang = SupportedLanguages.ContainsKey(systemLang) ? systemLang : "en";
                Instance._culture = new CultureInfo(effectiveLang);
            }
            else
            {
                Instance._culture = new CultureInfo(value);
            }
            Instance.OnPropertyChanged(null);
        }
    }

    /// <summary>
    /// Returns the actual language being used (resolves "System Default" to the actual language code)
    /// </summary>
    public static string EffectiveLanguage
    {
        get
        {
            var saved = Preferences.Get("AppLanguage", "");
            if (!string.IsNullOrEmpty(saved))
                return saved;
            
            // Resolve system language
            var systemLang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            return SupportedLanguages.ContainsKey(systemLang) ? systemLang : "en";
        }
    }

    public static bool IsZalgoMode
    {
        get => Instance._zalgoMode;
        set
        {
            Instance._zalgoMode = value;
            Preferences.Set("ZalgoMode", value);
            Instance.OnPropertyChanged(null);
        }
    }

    public string this[string key] => GetString(key);

    public static string Zalgoify(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        
        var result = new System.Text.StringBuilder();
        bool inPlaceholder = false;
        
        foreach (char c in text)
        {
            // Track if we're inside a format placeholder like {0}
            if (c == '{') inPlaceholder = true;
            else if (c == '}') inPlaceholder = false;
            
            result.Append(c);
            
            // Only add zalgo to letters/digits outside of placeholders
            if (!inPlaceholder && c != '}' && char.IsLetterOrDigit(c))
            {
                // Add 0-2 combining characters above, middle, and below
                for (int i = 0; i < _random.Next(0, 3); i++)
                    result.Append(ZalgoUp[_random.Next(ZalgoUp.Length)]);
                for (int i = 0; i < _random.Next(0, 2); i++)
                    result.Append(ZalgoMid[_random.Next(ZalgoMid.Length)]);
                for (int i = 0; i < _random.Next(0, 3); i++)
                    result.Append(ZalgoDown[_random.Next(ZalgoDown.Length)]);
            }
        }
        return result.ToString();
    }

    // Break message variations - randomly picks one
    private static readonly string[] BreakMessageKeys = { "TakeABreak", "TakeANap", "GrabACoffee", "StretchABit", "StareAtCeiling" };
    
    private string GetRandomBreakMessage()
    {
        var key = BreakMessageKeys[_random.Next(BreakMessageKeys.Length)];
        return this[key];
    }

    public static string GetString(string key)
    {
        var text = _resourceManager.GetString(key, Instance._culture) ?? key;
        // When zalgo mode is on, apply randomly 8% of the time for readability
        // When off, never apply zalgo
        var applyZalgo = Instance._zalgoMode && _random.Next(100) < 8;
        return applyZalgo ? Zalgoify(text) : text;
    }

    public static string GetString(string key, params object[] args)
    {
        var format = GetString(key);
        return string.Format(format, args);
    }

    // For backwards compatibility
    public static string Get(string key) => GetString(key);
    public static string Get(string key, params object[] args) => GetString(key, args);

    /// <summary>
    /// Refresh all string bindings to recompute zalgo randomness
    /// Call this on navigation, button clicks, etc.
    /// </summary>
    public static void Refresh()
    {
        Instance.OnPropertyChanged(null);
    }

    protected void OnPropertyChanged(string? propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Properties for direct XAML binding
    public string AppName => this["AppName"];
    public string Settings => this["Settings"];
    public string TabTasks => this["TabTasks"];
    public string TabGames => this["TabGames"];
    public string TabExcuses => this["TabExcuses"];
    public string TabStats => this["TabStats"];
    public string Accessibility => this["Accessibility"];
    public string HighContrastMode => this["HighContrastMode"];
    public string HighContrastDesc => this["HighContrastDesc"];
    public string DefaultTheme => this["DefaultTheme"];
    public string HighContrast => this["HighContrast"];
    public string ZalgoMode => this["ZalgoMode"];
    public string ZalgoModeDesc => this["ZalgoModeDesc"];
    public string ThemePreview => this["ThemePreview"];
    public string ChangesApply => this["ChangesApply"];
    public string Language => this["Language"];
    public string TodaysTasks => this["TodaysTasks"];
    public string YourProductivityList => this["YourProductivityList"];
    public string TakeABreak => GetRandomBreakMessage();
    public string DoingGreat => this["DoingGreat"];
    public string AddMoreTasks => this["AddMoreTasks"];
    public string Congratulations => this["Congratulations"];
    public string NeedAnotherBreak => this["NeedAnotherBreak"];
    public string MiniGames => this["MiniGames"];
    public string ProductivityOverrated => this["ProductivityOverrated"];
    public string ShuffleGames => this["ShuffleGames"];
    public string ExcuseGenerator => this["ExcuseGenerator"];
    public string NeedAReason => this["NeedAReason"];
    public string TapForExcuse => this["TapForExcuse"];
    public string GenerateExcuse => this["GenerateExcuse"];
    public string CopyToClipboard => this["CopyToClipboard"];
    public string YourStats => this["YourStats"];
    public string BeProud => this["BeProud"];
    public string TasksAvoided => this["TasksAvoided"];
    public string BreaksTaken => this["BreaksTaken"];
    public string ExcusesGeneratedStat => this["ExcusesGeneratedStat"];
    public string GamesPlayed => this["GamesPlayed"];
    public string TotalClicks => this["TotalClicks"];
    public string AIExcuseCalls => this["AIExcuseCalls"];
    public string AchievementUnlocked => this["AchievementUnlocked"];
    
    // Game strings
    public string ClickMe => this["ClickMe"];
    public string StartChallenge => this["StartChallenge"];
    public string Wait => this["Wait"];
    public string Start => this["Start"];
    public string TapStartToBegin => this["TapStartToBegin"];
    public string StartGame => this["StartGame"];
    public string NewGame => this["NewGame"];
    public string ThinkingOfNumber => this["ThinkingOfNumber"];
    public string EnterGuess => this["EnterGuess"];
    public string Guess => this["Guess"];
    public string YourTurn => this["YourTurn"];
    public string TiltToMove => this["TiltToMove"];
    public string StartGame30s => this["StartGame30s"];
    public string Generate => this["Generate"];
    
    // Excuse engine settings
    public string ExcuseEngine => this["ExcuseEngine"];
    public string ExcuseEngineDesc => this["ExcuseEngineDesc"];
    public string RandomGenerator => this["RandomGenerator"];
    public string CloudAI => this["CloudAI"];
    public string ApiEndpoint => this["ApiEndpoint"];
    public string ApiEndpointPlaceholder => this["ApiEndpointPlaceholder"];
    public string AiModel => this["AiModel"];
    public string Generating => this["Generating"];
    public string GroqApiKeyLabel => this["GroqApiKeyLabel"];
    public string GroqApiKeyPlaceholder => this["GroqApiKeyPlaceholder"];
    public string GroqGetKeyHint => this["GroqGetKeyHint"];
    public string OnDeviceAI => this["OnDeviceAI"];
    public string OnDeviceAIHint => this["OnDeviceAIHint"];
    public string OnDeviceAIAvailable => this["OnDeviceAIAvailable"];
    public string OnDeviceAIUnavailable => this["OnDeviceAIUnavailable"];
    
    // About section
    public string About => this["About"];
    public string AboutDescription => this["AboutDescription"];
    public string Author => this["Author"];
}
