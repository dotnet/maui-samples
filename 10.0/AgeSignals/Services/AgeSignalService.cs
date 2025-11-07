using AgeSignals.Models;

namespace AgeSignals.Services;

/// <summary>
/// Default implementation for unsupported platforms (e.g., Tizen)
/// This file is only compiled when none of the platform-specific files match
/// </summary>
public partial class AgeSignalService : IAgeSignalService
{
    public bool IsSupported() => false;

    public string GetPlatformName() => "Unsupported";

    public Task<AgeVerificationResult> RequestAgeVerificationAsync(int minimumAge = 13, object? platformContext = null)
    {
        return Task.FromResult(AgeVerificationResult.Failure("Age verification is not supported on this platform"));
    }

    public Task<AgeVerificationResult> RequestAgeVerificationAsync(AgeVerificationRequest request)
    {
        return Task.FromResult(AgeVerificationResult.Failure("Age verification is not supported on this platform"));
    }
}
