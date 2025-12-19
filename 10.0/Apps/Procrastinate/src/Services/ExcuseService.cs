namespace procrastinate.Services;

public class ExcuseService
{
    private readonly Dictionary<string, IExcuseGenerator> _generators;
    private readonly StatsService _stats;
    
    public ExcuseService(StatsService stats)
    {
        _stats = stats;
        _generators = new Dictionary<string, IExcuseGenerator>
        {
            { "random", new RandomExcuseGenerator() },
            { "cloud", new CloudExcuseGenerator() },
            { "ondevice", new OnDeviceAIExcuseGenerator() }
        };
    }

    public static string CurrentMode
    {
        get => Preferences.Get("ExcuseMode", "random");
        set => Preferences.Set("ExcuseMode", value);
    }

    public static Dictionary<string, string> AvailableModes
    {
        get
        {
            var modes = new Dictionary<string, string>
            {
                { "random", "Random Generator" },
                { "cloud", "Cloud AI (Groq)" }
            };
            
            // Apple Intelligence only available on iOS/macOS
#if IOS || MACCATALYST
            modes.Add("ondevice", "On-Device AI (Apple)");
#endif
            return modes;
        }
    }

    public IExcuseGenerator GetCurrentGenerator()
    {
        var mode = CurrentMode;
        return _generators.TryGetValue(mode, out var generator) ? generator : _generators["random"];
    }

    public async Task<ExcuseResult> GenerateExcuseAsync(string language)
    {
        var generator = GetCurrentGenerator();
        var usingAI = (CurrentMode == "cloud" || CurrentMode == "ondevice") && generator.IsAvailable;
        
        var result = await generator.GenerateExcuseAsync(language);
        
        if (usingAI)
        {
            _stats.IncrementAIExcuseCalls();
        }
        
        return result;
    }
}
