using AgeSignals.Models;
using AgeSignals.Services;
using Microsoft.Extensions.Logging;
using Windows.System;

namespace AgeSignals.Services;

public partial class AgeSignalService : IAgeSignalService
{
    private readonly ILogger<AgeSignalService> _logger;

    public AgeSignalService(ILogger<AgeSignalService> logger)
    {
        _logger = logger;
    }

    public string GetPlatformName() => "Windows";

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
        try
        {
            var users = await User.FindAllAsync();
            var currentUser = users.FirstOrDefault();

            if (currentUser == null)
            {
                _logger.LogWarning("No user logged in");
                return AgeVerificationResult.Failure("No user logged in");
            }

            // Check Adult first, then Minor, then Child to determine exact age group
            // Per Microsoft docs: Adult ⊃ Minor ⊃ Child (higher groups include lower groups)
            var adultResult = await currentUser.CheckUserAgeConsentGroupAsync(UserAgeConsentGroup.Adult);

            _logger.LogInformation("Age consent check result (Adult): {Result}", adultResult);

            if (adultResult == UserAgeConsentResult.Included)
            {
                return new AgeVerificationResult
                {
                    IsSuccess = true,
                    MinAge = 18,
                    MaxAge = null,
                    VerificationMethod = AgeVerificationMethod.WindowsAgeConsent
                };
            }

            // If not adult, check Minor group
            var minorResult = await currentUser.CheckUserAgeConsentGroupAsync(UserAgeConsentGroup.Minor);

            _logger.LogInformation("Age consent check result (Minor): {Result}", minorResult);

            if (minorResult == UserAgeConsentResult.Included)
            {
                return new AgeVerificationResult
                {
                    IsSuccess = request.MinimumAge <= 13, // Minors are typically 13-17
                    MinAge = 13,
                    MaxAge = 17,
                    VerificationMethod = AgeVerificationMethod.WindowsAgeConsent,
                    ErrorMessage = request.MinimumAge > 13 
                        ? $"Age requirement not met. Required: {request.MinimumAge}+, User is Minor (13-17)"
                        : null
                };
            }

            var childResult = await currentUser.CheckUserAgeConsentGroupAsync(UserAgeConsentGroup.Child);

            _logger.LogInformation("Age consent check result (Child): {Result}", childResult);

            switch (childResult)
            {
                case UserAgeConsentResult.Included:
                    return new AgeVerificationResult
                    {
                        IsSuccess = request.MinimumAge < 13,
                        MinAge = 0,
                        MaxAge = 12,
                        VerificationMethod = AgeVerificationMethod.WindowsAgeConsent,
                        ErrorMessage = request.MinimumAge >= 13
                            ? $"Age requirement not met. Required: {request.MinimumAge}+, User is Child (under 13)"
                            : null
                    };

                case UserAgeConsentResult.NotIncluded:
                    return AgeVerificationResult.Failure(
                        "User does not belong to any age consent group");

                case UserAgeConsentResult.Unknown:
                    return AgeVerificationResult.Failure(
                        "Age information not available.\n" +
                        "User should configure age in Microsoft account settings.");

                case UserAgeConsentResult.NotEnforced:
                    return AgeVerificationResult.Failure(
                        "Age consent groups not enforced in this region");

                case UserAgeConsentResult.Ambiguous:
                    return AgeVerificationResult.Failure(
                        "Age consent group information is ambiguous");

                default:
                    return AgeVerificationResult.Failure("Unable to determine age consent group");
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "Access denied - UserAccountInformation capability may be missing");
            return AgeVerificationResult.Failure(
                "Access denied.\n" +
                "Ensure UserAccountInformation capability is declared in Package.appxmanifest");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Age Consent API");
            return AgeVerificationResult.Failure($"Error: {ex.Message}");
        }
    }
}
