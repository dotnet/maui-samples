namespace AgeSignals.Models;

/// <summary>
/// Indicates how the user's age was verified
/// </summary>
public enum AgeVerificationMethod
{
    /// <summary>
    /// Unknown or not specified
    /// </summary>
    Unknown,

    /// <summary>
    /// User declared their own age
    /// </summary>
    SelfDeclared,

    /// <summary>
    /// Guardian declared the user's age
    /// </summary>
    GuardianDeclared,

    /// <summary>
    /// Platform-verified (e.g., through account settings)
    /// </summary>
    PlatformVerified,

    /// <summary>
    /// Third-party verification
    /// </summary>
    ThirdParty,

    /// <summary>
    /// Google Play Age Signals (Android)
    /// </summary>
    GooglePlay,

    /// <summary>
    /// Apple Declared Age Range (iOS only)
    /// </summary>
    AppleDeclaredAgeRange,

    /// <summary>
    /// Windows User Age Consent (Windows)
    /// </summary>
    WindowsAgeConsent
}
