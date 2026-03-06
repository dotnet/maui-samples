using System.Diagnostics;
using Microsoft.Extensions.AI;

namespace procrastinate.Services;

/// <summary>
/// On-device AI excuse generator using Apple Intelligence via MEAI's AppleIntelligenceChatClient (IChatClient).
/// Requires iOS 26+ / macOS 26+ with Apple Intelligence enabled.
/// </summary>
public class OnDeviceAIExcuseGenerator : IExcuseGenerator
{
    private readonly IChatClient? _chatClient;

    public string Name => "On-Device AI (Apple)";

    public static readonly string[] RandomTopics =
    [
        "a pet", "the weather", "technology", "food", "sleep",
        "aliens", "a mysterious stranger", "time travel", "gravity",
        "a horoscope", "a dream", "a fortune cookie", "the internet",
        "a conspiracy theory", "a cosmic event", "socks", "a squirrel",
        "a parallel universe", "a broken clock", "invisible forces",
        "a wizard", "quantum physics", "a haunted toaster", "yoga",
        "a pigeon", "my neighbor's cat", "a banana peel", "magnets",
        "a black hole", "my goldfish", "a rogue umbrella", "quicksand",
        "a cursed elevator", "my WiFi", "a haunted parking lot",
        "a sentient roomba", "a possessed microwave", "my shoes",
        "a time-traveling snail", "a dancing cactus", "my shadow",
        "a suspicious cloud", "an enchanted burrito", "a talking fridge",
        "a rogue shopping cart", "my evil twin", "a philosophical duck",
        "a vampire accountant", "a teleporting sandwich", "solar flares",
        "my karma", "a runaway flamingo", "an angry seagull",
        "a melodramatic houseplant", "a judgmental owl", "moon phases",
        "a passive-aggressive GPS", "a ninja hamster", "a broken zipper",
        "a vengeful parking meter", "an existential crisis", "a glitch in the matrix"
    ];

    public static readonly string[] StarterPhrases =
    [
        "I can't because", "I would but", "Sorry, unfortunately",
        "I tried, however", "I'd love to, but", "It's impossible since",
        "I must decline because", "Not today because", "I was going to, but"
    ];

    public OnDeviceAIExcuseGenerator(IChatClient? chatClient = null)
    {
        _chatClient = chatClient;
    }

    public bool IsAvailable => _chatClient is not null;

    public async Task<ExcuseResult> GenerateExcuseAsync(string language)
    {
        var stopwatch = Stopwatch.StartNew();

        if (_chatClient is null)
        {
            stopwatch.Stop();
            return new ExcuseResult("On-device AI is not available on this device.", Name, stopwatch.Elapsed);
        }

        try
        {
            var languageName = language switch
            {
                "fr" => "French",
                "es" => "Spanish",
                "pt" => "Portuguese",
                "nl" => "Dutch",
                "cs" => "Czech",
                _ => "English"
            };

            // Randomize the prompt so the on-device model doesn't return the same answer
            var topic = RandomTopics[Random.Shared.Next(RandomTopics.Length)];
            var starter = StarterPhrases[Random.Shared.Next(StarterPhrases.Length)];

            var prompt = $"Write a single short humorous fictional excuse about {topic}. Start with '{starter}'. Make it silly and absurd. Write it in {languageName}. Just the excuse, nothing else.";

            var messages = new List<ChatMessage>
            {
                new(ChatRole.User, prompt)
            };

            var response = await _chatClient.GetResponseAsync(messages);
            stopwatch.Stop();

            var excuse = response.Text?.Trim() ?? "The on-device AI is also procrastinating...";

            return new ExcuseResult(excuse, Name, stopwatch.Elapsed, Model: "Apple Intelligence");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            System.Diagnostics.Debug.WriteLine($"On-device AI error: {ex}");
            return new ExcuseResult("The on-device AI got stage fright. Give it another try! 🎭", Name, stopwatch.Elapsed);
        }
    }
}
