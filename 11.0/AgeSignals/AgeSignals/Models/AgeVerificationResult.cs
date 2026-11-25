namespace AgeSignals.Models;

/// <summary>
/// Represents the result of an age verification request
/// </summary>
public class AgeVerificationResult
{
    /// <summary>
    /// Indicates if the age verification was successful
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Minimum age in the user's age range
    /// </summary>
    public int? MinAge { get; set; }

    /// <summary>
    /// Maximum age in the user's age range (null means no upper limit/infinity)
    /// </summary>
    public int? MaxAge { get; set; }

    /// <summary>
    /// How the age was verified
    /// </summary>
    public AgeVerificationMethod VerificationMethod { get; set; }

    /// <summary>
    /// Indicates if the user declined to share age information
    /// </summary>
    public bool UserDeclined { get; set; }

    /// <summary>
    /// User status from platform (e.g., VERIFIED, SUPERVISED, UNKNOWN)
    /// </summary>
    public string? UserStatus { get; set; }

    /// <summary>
    /// Install ID from Google Play (for supervised users)
    /// </summary>
    public string? InstallId { get; set; }

    /// <summary>
    /// Most recent approval date (for supervised users)
    /// </summary>
    public DateTime? ApprovalDate { get; set; }

    /// <summary>
    /// Raw JSON response from platform API (for debugging/logging)
    /// </summary>
    public string? RawJson { get; set; }

    /// <summary>
    /// Error message if the operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Platform-specific data
    /// </summary>
    public object? PlatformData { get; set; }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static AgeVerificationResult Success(int minAge, int? maxAge, AgeVerificationMethod method)
    {
        return new AgeVerificationResult
        {
            IsSuccess = true,
            MinAge = minAge,
            MaxAge = maxAge,
            VerificationMethod = method
        };
    }

    /// <summary>
    /// Creates a result when user declines to share age information
    /// </summary>
    public static AgeVerificationResult Declined()
    {
        return new AgeVerificationResult
        {
            IsSuccess = false,
            UserDeclined = true,
            ErrorMessage = "User declined to share age information"
        };
    }

    /// <summary>
    /// Creates a failed result
    /// </summary>
    public static AgeVerificationResult Failure(string errorMessage)
    {
        return new AgeVerificationResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}
