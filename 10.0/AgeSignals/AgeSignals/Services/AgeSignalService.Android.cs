using AgeSignals.Models;
using AgeSignals.Services;
using Microsoft.Extensions.Logging;
using Google.Android.Play.AgeSignals;
using Android.Gms.Tasks;
using Android.Gms.Common.Apis;
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
        try
        {
            var context = global::Android.App.Application.Context;
            if (context == null)
            {
                _logger.LogError("Unable to get Android context");
                return AgeVerificationResult.Failure("Unable to get Android context");
            }

            if (_ageSignalsManager == null)
            {
                _ageSignalsManager = AgeSignalsManagerFactory.Create(context);
                _logger.LogInformation("Age Signals Manager initialized");
            }

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
                    _logger.LogError("Age Signals API error: Type={ExceptionType}, Message={Error}", 
                        exception.GetType().FullName, exception.Message);
                    
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
        if (exception is Java.Lang.UnsupportedOperationException)
        {
            return $"Age Signals API is not supported on this device.\n{exception.Message}";
        }
        
        // Age Signals API may use the same error codes as Play Integrity API
        // Reference: https://developer.android.com/google/play/integrity/error-codes
        if (exception is ApiException apiException)
        {
            int statusCode = apiException.StatusCode;
            
            return statusCode switch
            {
                -1 => "Play Age Signals API not available.\n" +
                      "The Play Store version may be outdated.\n" +
                      "Ask user to update Google Play Store.",
                      
                -2 => "Google Play Store not found.\n" +
                      "Ask user to install or enable Play Store.",
                      
                -3 => "Network error.\n" +
                      "Check internet connection and try again.",
                      
                -6 => "Google Play Services not found.\n" +
                      "Ask user to install, update, or enable Play Services.",
                      
                -9 => "Cannot bind to Play Store service.\n" +
                      "Ask user to update Play Store and try again.",
                      
                -100 => "Internal error occurred.\n" +
                        "Please try again later.",
                        
                _ => $"Age Signals API error (code {statusCode}): {apiException.Message}"
            };
        }
        
        // Fallback: return the exception message
        return $"Age Signals API error: {exception.Message}";
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

