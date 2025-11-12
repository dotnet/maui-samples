using AgeSignals.Models;
using AgeSignals.Services;
using Microsoft.Extensions.Logging;
using Google.Android.Play.AgeSignals;
using Android.Gms.Tasks;
using Java.Lang;

namespace AgeSignals.Services;

public partial class AgeSignalService : IAgeSignalService
{
    private readonly ILogger<AgeSignalService> _logger;
    private IAgeSignalsManager? _ageSignalsManager;

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
            var context = global::Android.App.Application.Context;
            if (context == null)
            {
                _logger.LogError("Unable to get Android context");
                return AgeVerificationResult.Failure("Unable to get Android context");
            }

            // Initialize the Age Signals Manager if not already done
            if (_ageSignalsManager == null)
            {
                _ageSignalsManager = AgeSignalsManagerFactory.Create(context);
                _logger.LogInformation("Age Signals Manager initialized");
            }

            // Create the age signals request
            var ageSignalsRequest = AgeSignalsRequest.InvokeBuilder().Build();

            _logger.LogInformation("Requesting age signals...");

            // Use TaskCompletionSource to convert Java Task to C# Task
            var tcs = new TaskCompletionSource<AgeVerificationResult>();

            _ageSignalsManager.CheckAgeSignals(ageSignalsRequest)
                .AddOnSuccessListener(new AgeSignalsSuccessListener((ageSignalsResult) =>
                {
                    try
                    {
                        var userStatus = ageSignalsResult.UserStatus()?.IntValue() ?? 0;
                        var ageLower = ageSignalsResult.AgeLower()?.IntValue() ?? 0;
                        var ageUpper = ageSignalsResult.AgeUpper()?.IntValue() ?? 0;
                        var installId = ageSignalsResult.InstallId();
                        var approvalDate = ageSignalsResult.MostRecentApprovalDate();

                        _logger.LogInformation("Age Signals received: Status={Status}, Age={AgeLower}-{AgeUpper}, InstallID={InstallId}",
                            userStatus, ageLower, ageUpper, installId);

                        // Process the response based on user status
                        switch (userStatus)
                        {
                            case 1: // VERIFIED
                                // User is verified as 18+
                                tcs.SetResult(new AgeVerificationResult
                                {
                                    IsSuccess = true,
                                    MinAge = 18,
                                    MaxAge = null,
                                    VerificationMethod = AgeVerificationMethod.GooglePlay,
                                    UserStatus = "VERIFIED"
                                });
                                break;

                            case 2: // SUPERVISED
                                // User is supervised with age range
                                var meetsRequirement = ageLower >= request.MinimumAge;
                                tcs.SetResult(new AgeVerificationResult
                                {
                                    IsSuccess = meetsRequirement,
                                    MinAge = ageLower,
                                    MaxAge = ageUpper,
                                    VerificationMethod = AgeVerificationMethod.GooglePlay,
                                    UserStatus = "SUPERVISED",
                                    InstallId = installId,
                                    ApprovalDate = approvalDate != null ? System.DateTimeOffset.FromUnixTimeMilliseconds(approvalDate.Time).DateTime : null,
                                    ErrorMessage = meetsRequirement ? null : $"Age requirement not met. Required: {request.MinimumAge}+, User age: {ageLower}-{ageUpper}"
                                });
                                break;

                            case 3: // SUPERVISED_APPROVAL_PENDING
                                // Supervised user with pending approval
                                tcs.SetResult(new AgeVerificationResult
                                {
                                    IsSuccess = false,
                                    MinAge = ageLower,
                                    MaxAge = ageUpper,
                                    VerificationMethod = AgeVerificationMethod.GooglePlay,
                                    UserStatus = "SUPERVISED_APPROVAL_PENDING",
                                    InstallId = installId,
                                    ApprovalDate = approvalDate != null ? System.DateTimeOffset.FromUnixTimeMilliseconds(approvalDate.Time).DateTime : null,
                                    ErrorMessage = "Parent approval is pending for significant changes"
                                });
                                break;

                            case 4: // SUPERVISED_APPROVAL_DENIED
                                // Supervised user with denied approval
                                tcs.SetResult(new AgeVerificationResult
                                {
                                    IsSuccess = false,
                                    MinAge = ageLower,
                                    MaxAge = ageUpper,
                                    VerificationMethod = AgeVerificationMethod.GooglePlay,
                                    UserStatus = "SUPERVISED_APPROVAL_DENIED",
                                    InstallId = installId,
                                    ApprovalDate = approvalDate != null ? System.DateTimeOffset.FromUnixTimeMilliseconds(approvalDate.Time).DateTime : null,
                                    ErrorMessage = "Parent denied approval for significant changes"
                                });
                                break;

                            case 0: // UNKNOWN
                                // User status unknown
                                tcs.SetResult(AgeVerificationResult.Failure(
                                    "Age verification status unknown.\n" +
                                    "User may not be verified or supervised in applicable jurisdictions.\n" +
                                    "Ask user to visit Play Store to resolve their status."));
                                break;

                            default:
                                // Empty or other status
                                tcs.SetResult(AgeVerificationResult.Failure(
                                    "Age signals not available for this user.\n" +
                                    "User may not be in an applicable jurisdiction."));
                                break;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        _logger.LogError(ex, "Error processing age signals result");
                        tcs.SetResult(AgeVerificationResult.Failure($"Error processing result: {ex.Message}"));
                    }
                }))
                .AddOnFailureListener(new AgeSignalsFailureListener((exception) =>
                {
                    _logger.LogError("Age Signals API error: {Error}", exception.Message);
                    
                    // Map error codes to user-friendly messages
                    var errorMessage = MapErrorCodeToMessage(exception);
                    tcs.SetResult(AgeVerificationResult.Failure(errorMessage));
                }));

            return await tcs.Task;
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error calling Age Signals API");
            return AgeVerificationResult.Failure($"Error: {ex.Message}");
        }
    }

    private string MapErrorCodeToMessage(Java.Lang.Exception exception)
    {
        // Error codes from the API documentation
        var message = exception.Message ?? "";
        
        if (message.Contains("-1") || message.Contains("API_NOT_AVAILABLE"))
        {
            return "Play Age Signals API not available.\n" +
                   "The Play Store version may be outdated.\n" +
                   "Ask user to update Google Play Store.";
        }
        else if (message.Contains("-2") || message.Contains("PLAY_STORE_NOT_FOUND"))
        {
            return "Google Play Store not found.\n" +
                   "Ask user to install or enable Play Store.";
        }
        else if (message.Contains("-3") || message.Contains("NETWORK_ERROR"))
        {
            return "Network error.\n" +
                   "Check internet connection and try again.";
        }
        else if (message.Contains("-4") || message.Contains("PLAY_SERVICES_NOT_FOUND"))
        {
            return "Google Play Services not found.\n" +
                   "Ask user to install, update, or enable Play Services.";
        }
        else if (message.Contains("-5") || message.Contains("CANNOT_BIND_TO_SERVICE"))
        {
            return "Cannot bind to Play Store service.\n" +
                   "Play Store may need update or device memory is low.\n" +
                   "Ask user to update Play Store and try again.";
        }
        else if (message.Contains("-6") || message.Contains("PLAY_STORE_VERSION_OUTDATED"))
        {
            return "Google Play Store is outdated.\n" +
                   "Ask user to update Play Store.";
        }
        else if (message.Contains("-7") || message.Contains("PLAY_SERVICES_VERSION_OUTDATED"))
        {
            return "Google Play Services is outdated.\n" +
                   "Ask user to update Play Services.";
        }
        else if (message.Contains("-8") || message.Contains("CLIENT_TRANSIENT_ERROR"))
        {
            return "Transient error occurred.\n" +
                   "Please try again in a moment.";
        }
        else if (message.Contains("-9") || message.Contains("APP_NOT_OWNED"))
        {
            return "App not installed from Google Play Store.\n" +
                   "Age Signals API requires app from Play Store (not sideloaded).";
        }
        else if (message.Contains("-100") || message.Contains("INTERNAL_ERROR"))
        {
            return "Internal error occurred.\n" +
                   "Please try again later.";
        }
        
        return $"Age Signals API error: {message}";
    }
}

// Success listener implementation
internal class AgeSignalsSuccessListener : Java.Lang.Object, IOnSuccessListener
{
    private readonly Action<AgeSignalsResult> _onSuccess;

    public AgeSignalsSuccessListener(Action<AgeSignalsResult> onSuccess)
    {
        _onSuccess = onSuccess;
    }

    public void OnSuccess(Java.Lang.Object? result)
    {
        if (result is AgeSignalsResult ageSignalsResult)
        {
            _onSuccess(ageSignalsResult);
        }
    }
}

// Failure listener implementation
internal class AgeSignalsFailureListener : Java.Lang.Object, IOnFailureListener
{
    private readonly Action<Java.Lang.Exception> _onFailure;

    public AgeSignalsFailureListener(Action<Java.Lang.Exception> onFailure)
    {
        _onFailure = onFailure;
    }

    public void OnFailure(Java.Lang.Exception exception)
    {
        _onFailure(exception);
    }
}

