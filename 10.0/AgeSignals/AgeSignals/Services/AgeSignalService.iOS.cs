using AgeSignals.Models;
using AgeSignals.Services;
using Microsoft.Extensions.Logging;
using System.Linq;
using UIKit;
using DeclaredAgeRangeWrapper;

namespace AgeSignals.Services;

public partial class AgeSignalService : IAgeSignalService
{
    private readonly ILogger<AgeSignalService> _logger;

    public AgeSignalService(ILogger<AgeSignalService> logger)
    {
        _logger = logger;
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
        var tcs = new TaskCompletionSource<AgeVerificationResult>();

        try
        {
            var anchor = request.PlatformContext as UIViewController;
            
            if (anchor == null)
            {
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
                    _logger.LogError("Age verification error: Domain={Domain}, Code={Code}, Message={Message}", 
                        error.Domain, error.Code, error.LocalizedDescription);
                    
                    string userMessage = MapIOSErrorCodeToMessage(error.Code, error.LocalizedDescription);
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
                                var lowerBound = (int)(response.Range.LowerBound?.Int32Value ?? 0);
                                var upperBound = response.Range.UpperBound?.Int32Value;

                                _logger.LogInformation("Age range received: {Lower}-{Upper}, Declaration: {Declaration}", 
                                    lowerBound, upperBound?.ToString() ?? "∞", response.Range.Declaration);

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

    private string MapIOSErrorCodeToMessage(nint errorCode, string errorMsg)
    {
        return $"Declared Age Range API error (Code {errorCode}):\n{errorMsg}";
    }
}
