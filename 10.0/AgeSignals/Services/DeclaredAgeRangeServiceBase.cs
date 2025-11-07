using AgeSignals.Models;
using Microsoft.Extensions.Logging;

namespace AgeSignals.Services;

public abstract class DeclaredAgeRangeServiceBase : IAgeSignalService
{
    protected readonly ILogger<DeclaredAgeRangeServiceBase> _logger;
    protected abstract string PlatformName { get; }

    protected DeclaredAgeRangeServiceBase(ILogger<DeclaredAgeRangeServiceBase> logger)
    {
        _logger = logger;
    }

    public abstract bool IsSupported();

    public string GetPlatformName() => PlatformName;

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
            return AgeVerificationResult.Failure($"Declared Age Range API requires {PlatformName} 18.0+ / macOS 15.0+");
        }

        try
        {
            // Note: Real implementation requires:
            // 1. Entitlement in Entitlements.plist: com.apple.developer.declared-age-range
            // 2. Swift XCFramework wrapper (see: https://github.com/dalexsoto/DeclaredAgeRangeSample)
            // 3. Only works on physical devices (not simulator)
            // 4. User must have age set in Apple ID settings
            
            _logger.LogInformation("Declared Age Range API not yet implemented - requires Swift wrapper");
            
            await Task.Delay(100);
            return AgeVerificationResult.Failure(
                $"Declared Age Range API requires:\n" +
                $"• {PlatformName} 18.0+ / macOS 15.0+\n" +
                $"• Physical device (not simulator)\n" +
                $"• User age set in Apple ID settings\n" +
                $"• Swift XCFramework wrapper implementation");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Declared Age Range API");
            return AgeVerificationResult.Failure($"Error: {ex.Message}");
        }
    }
}
