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

    public bool IsSupported()
    {
        return OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000);
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
        if (!IsSupported())
        {
            return AgeVerificationResult.Failure("Windows Age Consent API requires Windows 11 (Build 22000+)");
        }

        try
        {
            var users = await User.FindAllAsync();
            var currentUser = users.FirstOrDefault();

            if (currentUser == null)
            {
                _logger.LogWarning("No user logged in");
                return AgeVerificationResult.Failure("No user logged in");
            }

            // Check Adult group first (18+)
            var adultResult = await currentUser.CheckUserAgeConsentGroupAsync(UserAgeConsentGroup.Adult);

            _logger.LogInformation("Age consent check result: {Result}", adultResult);

            switch (adultResult)
            {
                case UserAgeConsentResult.Included:
                    return new AgeVerificationResult
                    {
                        IsSuccess = true,
                        MinAge = 18,
                        MaxAge = null,
                        VerificationMethod = AgeVerificationMethod.WindowsAgeConsent
                    };

                case UserAgeConsentResult.NotIncluded:
                    // Not adult, check Child group
                    var childResult = await currentUser.CheckUserAgeConsentGroupAsync(UserAgeConsentGroup.Child);
                    
                    switch (childResult)
                    {
                        case UserAgeConsentResult.Included:
                            return new AgeVerificationResult
                            {
                                IsSuccess = true,
                                MinAge = 0,
                                MaxAge = 17,
                                VerificationMethod = AgeVerificationMethod.WindowsAgeConsent
                            };
                        
                        case UserAgeConsentResult.Unknown:
                            _logger.LogWarning("Age information not available");
                            return AgeVerificationResult.Failure(
                                "Age information not available.\n" +
                                "User should configure age in Microsoft account settings.");
                        
                        case UserAgeConsentResult.NotEnforced:
                            _logger.LogInformation("Age consent not enforced in this region");
                            return AgeVerificationResult.Failure(
                                "Age consent groups not enforced in this region");
                        
                        default:
                            return AgeVerificationResult.Failure("Unable to determine child age consent group");
                    }

                case UserAgeConsentResult.Unknown:
                    _logger.LogWarning("Age information not available");
                    return AgeVerificationResult.Failure(
                        "Age information not available.\n" +
                        "User should configure age in Microsoft account settings.");

                case UserAgeConsentResult.NotEnforced:
                    _logger.LogInformation("Age consent not enforced in this region");
                    return AgeVerificationResult.Failure(
                        "Age consent groups not enforced in this region");

                case UserAgeConsentResult.Ambiguous:
                    _logger.LogWarning("Age consent group is ambiguous");
                    return AgeVerificationResult.Failure(
                        "Age consent group information is ambiguous");
            }

            return AgeVerificationResult.Failure("Unable to determine age consent group");
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
