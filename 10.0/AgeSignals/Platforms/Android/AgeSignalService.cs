using AgeSignals.Models;
using Microsoft.Extensions.Logging;

namespace AgeSignals.Services;

public partial class AgeSignalService : IAgeSignalService
{
    private readonly ILogger<AgeSignalService> _logger;

    public AgeSignalService(ILogger<AgeSignalService> logger)
    {
        _logger = logger;
    }

    public bool IsSupported()
    {
        return OperatingSystem.IsAndroidVersionAtLeast(23);
    }

    public string GetPlatformName() => "Android";

    public Task<AgeVerificationResult> RequestAgeVerificationAsync(int minimumAge = 13, object? platformContext = null)
    {
        return RequestAgeVerificationAsync(new AgeVerificationRequest
        {
            MinimumAge = minimumAge,
            PlatformContext = platformContext
        });
    }

    public async Task<AgeVerificationResult> RequestAgeVerificationAsync(AgeVerificationRequest request)
    {
        if (!IsSupported())
        {
            return AgeVerificationResult.Failure("Age Signals API requires Android 6.0+ (API 23)");
        }

        try
        {
            var context = Android.App.Application.Context;
            if (context == null)
            {
                _logger.LogError("Unable to get Android context");
                return AgeVerificationResult.Failure("Unable to get Android context");
            }

            // Note: Real implementation requires:
            // 1. NuGet package: Xamarin.Google.Android.Play.Age.Signals v0.0.1-beta02
            // 2. App distributed through Google Play Store
            // 3. User in applicable jurisdiction (e.g., certain US states)
            // 4. Date after January 1, 2026 (API goes live)
            
            _logger.LogInformation("Age Signals API not yet implemented - requires Play Store distribution");
            
            await Task.Delay(100);
            return AgeVerificationResult.Failure(
                "Age Signals API requires:\n" +
                "• App from Google Play Store (not sideloaded)\n" +
                "• Applicable jurisdiction (e.g., certain US states)\n" +
                "• Date after January 1, 2026\n" +
                "• User age verified in Play Store settings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Age Signals API");
            return AgeVerificationResult.Failure($"Error: {ex.Message}");
        }
    }
}
