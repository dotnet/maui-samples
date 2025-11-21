using AgeSignals.Models;

namespace AgeSignals.Services;

/// <summary>
/// Service interface for age verification across different platforms
/// </summary>
public interface IAgeSignalService
{
    /// <summary>
    /// Requests age verification from the user
    /// </summary>
    /// <param name="minimumAge">The minimum age required</param>
    /// <param name="platformContext">Platform-specific context (UIViewController, Activity, etc.)</param>
    /// <returns>The age verification result</returns>
    Task<AgeVerificationResult> RequestAgeVerificationAsync(int minimumAge = 13, object? platformContext = null);

    /// <summary>
    /// Requests age verification with detailed parameters
    /// </summary>
    /// <param name="request">The age verification request parameters</param>
    /// <returns>The age verification result</returns>
    Task<AgeVerificationResult> RequestAgeVerificationAsync(AgeVerificationRequest request);

    /// <summary>
    /// Gets the platform name for the current implementation
    /// </summary>
    /// <returns>Platform name (iOS, Android, Windows)</returns>
    string GetPlatformName();
}
