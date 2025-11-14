using AgeSignals.Models;
using AgeSignals.Services;
using Microsoft.Extensions.Logging;
using System.Linq;
using UIKit;
using DeclaredAgeRangeWrapper;
using Foundation;

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
        return OperatingSystem.IsIOSVersionAtLeast(26, 0);
    }

    public string GetPlatformName()
    {
        return "iOS";
    }

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
            return AgeVerificationResult.Failure("Declared Age Range API requires iOS 26.0+");
        }

        var tcs = new TaskCompletionSource<AgeVerificationResult>();

        try
        {
            // iOS: Use UIViewController - get from PlatformContext or find root view controller
            var anchor = request.PlatformContext as UIViewController;
            
            if (anchor == null)
            {
                // Try to get the root view controller from the key window
                var windowScene = UIApplication.SharedApplication.ConnectedScenes
                    .OfType<UIWindowScene>()
                    .FirstOrDefault(scene => scene.ActivationState == UISceneActivationState.ForegroundActive);
                
                anchor = windowScene?.Windows.FirstOrDefault(w => w.IsKeyWindow)?.RootViewController;
            }

            if (anchor == null)
            {
                _logger.LogError("Unable to get UIViewController for age verification");
                return AgeVerificationResult.Failure("Unable to get UIViewController. Ensure the app is running with a visible window.");
            }

            _logger.LogInformation("Requesting age range with minimum age: {MinimumAge}", request.MinimumAge);

            nint minimumAge = request.MinimumAge;
            DeclaredAgeRangeBridge.RequestAgeRange(minimumAge, null, null, anchor, (response, error) =>
            {
                if (error != null)
                {
                    var errorCode = error.Code;
                    var errorMsg = error.LocalizedDescription;
                    
                    _logger.LogError("Age verification error: Code={Code}, Message={Message}", errorCode, errorMsg);
                    
                    // Provide helpful error messages
                    string userMessage;
                    if (errorCode == 0 || errorMsg.Contains("not supported", StringComparison.OrdinalIgnoreCase))
                    {
                        userMessage = 
                            "Declared Age Range Not Available\n\n" +
                            "This error typically means:\n" +
                            "- Running in Simulator (API only works on physical devices)\n" +
                            "- iOS version < 18.0\n" +
                            "- No age configured in Apple ID Settings\n" +
                            "- App not properly signed with entitlement\n\n" +
                            $"Error: {errorMsg}";
                    }
                    else
                    {
                        userMessage = $"Error ({errorCode}): {errorMsg}";
                    }
                    
                    tcs.SetResult(AgeVerificationResult.Failure(userMessage));
                    return;
                }

                if (response == null)
                {
                    _logger.LogError("No response received from age verification");
                    tcs.SetResult(AgeVerificationResult.Failure("No response received"));
                        return;
                    }

                    switch (response.Type)
                    {
                        case MyAgeRangeResponseType.Sharing:
                            if (response.Range != null)
                            {
                                // NSNumber properties are already nullable - access IntValue property directly
                                var lowerBound = (int)(response.Range.LowerBound?.Int32Value ?? 0);
                                var upperBound = response.Range.UpperBound?.Int32Value;
                                var declaration = response.Range.Declaration;

                                _logger.LogInformation("Age range received: {Lower}-{Upper}, Declaration: {Declaration}", 
                                    lowerBound, upperBound?.ToString() ?? "∞", declaration);

                                var meetsRequirement = lowerBound >= request.MinimumAge;
                                tcs.SetResult(new AgeVerificationResult
                                {
                                    IsSuccess = meetsRequirement,
                                    VerificationMethod = AgeVerificationMethod.AppleDeclaredAgeRange,
                                    MinAge = lowerBound,
                                    MaxAge = upperBound,
                                    ErrorMessage = meetsRequirement 
                                        ? null
                                        : $"Age requirement not met. Required: {request.MinimumAge}+, Declared: {lowerBound}-{upperBound?.ToString() ?? "∞"}"
                                });
                            }
                            else
                            {
                                tcs.SetResult(AgeVerificationResult.Failure("Age range data missing"));
                            }
                            break;

                        case MyAgeRangeResponseType.DeclinedSharing:
                            _logger.LogInformation("User declined to share age information");
                            tcs.SetResult(AgeVerificationResult.Failure("User declined to share age information"));
                            break;

                        default:
                            tcs.SetResult(AgeVerificationResult.Failure("Unknown response type"));
                            break;
                    }
                });

            return await tcs.Task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting age verification");
            return AgeVerificationResult.Failure($"Error: {ex.Message}");
        }
    }
}
