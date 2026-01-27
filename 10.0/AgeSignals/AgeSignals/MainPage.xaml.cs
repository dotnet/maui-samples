
using AgeSignals.Services;
namespace AgeSignals;
public partial class MainPage : ContentPage
{
    private readonly IAgeSignalService _ageSignalService;
    public MainPage(IAgeSignalService ageSignalService)
    {
        InitializeComponent();
        _ageSignalService = ageSignalService;
        
        // Set platform information
        PlatformLabel.Text = $"Platform: {_ageSignalService.GetPlatformName()}";
    }
    private async void OnCheckAgeClicked(object sender, EventArgs e)
    {
        try
        {
            // Show loading state
            CheckAgeButton.IsEnabled = false;
            CheckAgeButton.Text = "Verifying...";
            ResultBorder.IsVisible = false;
            var result = await _ageSignalService.RequestAgeVerificationAsync(13, null);
            // Show result border
            ResultBorder.IsVisible = true;
            if (result.IsSuccess)
            {
                // Success state
                ResultTitleLabel.Text = "Age Verification Successful";
                ResultTitleLabel.TextColor = Color.FromArgb("#28a745");
                
                var maxAge = result.MaxAge.HasValue ? result.MaxAge.Value.ToString() : "∞";
                ResultLabel.Text = $"Age Range: {result.MinAge} - {maxAge} years\n" +
                                 $"Verification Method: {result.VerificationMethod}";
                ResultLabel.TextColor = Color.FromArgb("#333333");
                
                ResultBorder.Stroke = Color.FromArgb("#28a745");
            }
            else if (result.UserDeclined)
            {
                // User declined
                ResultTitleLabel.Text = "User Declined";
                ResultTitleLabel.TextColor = Color.FromArgb("#ff8c00");
                ResultLabel.Text = result.ErrorMessage ?? "User chose not to share age information";
                ResultLabel.TextColor = Color.FromArgb("#666666");
                ResultBorder.Stroke = Color.FromArgb("#ff8c00");
            }
            else
            {
                // Error state
                ResultTitleLabel.Text = "Verification Unavailable";
                ResultTitleLabel.TextColor = Color.FromArgb("#dc3545");
                ResultLabel.Text = result.ErrorMessage ?? "Unable to verify age";
                ResultLabel.TextColor = Color.FromArgb("#666666");
                ResultBorder.Stroke = Color.FromArgb("#dc3545");
            }
        }
        catch (Exception ex)
        {
            // Exception handling
            ResultBorder.IsVisible = true;
            ResultTitleLabel.Text = "Error Occurred";
            ResultTitleLabel.TextColor = Color.FromArgb("#dc3545");
            ResultLabel.Text = $"{ex.Message}\n\nPlease ensure all platform-specific requirements are met.";
            ResultLabel.TextColor = Color.FromArgb("#666666");
            ResultBorder.Stroke = Color.FromArgb("#dc3545");
        }
        finally
        {
            // Reset button state
            CheckAgeButton.IsEnabled = true;
            CheckAgeButton.Text = "Verify Age";
        }
    }
}