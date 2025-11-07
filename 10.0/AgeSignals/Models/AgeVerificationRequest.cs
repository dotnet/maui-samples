namespace AgeSignals.Models;

/// <summary>
/// Request parameters for age verification
/// </summary>
public class AgeVerificationRequest
{
    /// <summary>
    /// Required minimum age for the feature/content
    /// </summary>
    public int MinimumAge { get; set; } = 13;

    /// <summary>
    /// Optional secondary age threshold
    /// </summary>
    public int? SecondaryAgeThreshold { get; set; }

    /// <summary>
    /// Optional tertiary age threshold
    /// </summary>
    public int? TertiaryAgeThreshold { get; set; }

    /// <summary>
    /// Platform-specific context (e.g., UIViewController for iOS, Activity for Android)
    /// </summary>
    public object? PlatformContext { get; set; }

    /// <summary>
    /// Timeout for the verification request in milliseconds
    /// </summary>
    public int TimeoutMs { get; set; } = 30000;
}
