using AgeSignals.Services;

namespace AgeSignals;

public partial class MainPage : ContentPage
{
    private readonly IAgeSignalService _ageSignalService;

    public MainPage(IAgeSignalService ageSignalService)
    {
        InitializeComponent();
        _ageSignalService = ageSignalService;
        
        PlatformLabel.Text = $"Platform: {_ageSignalService.GetPlatformName()}";
        SupportedLabel.Text = $"Supported: {(_ageSignalService.IsSupported() ? "✓ Yes" : "✗ No")}";
    }

    private async void OnCheckAgeClicked(object sender, EventArgs e)
    {
        try
        {
            ResultLabel.Text = "Checking age signals...";
            ResultLabel.TextColor = Colors.Gray;
            CheckAgeButton.IsEnabled = false;

            var result = await _ageSignalService.RequestAgeVerificationAsync(13, null);

            if (result.IsSuccess)
            {
                var maxAge = result.MaxAge.HasValue ? result.MaxAge.Value.ToString() : "∞";
                ResultLabel.Text = $"✓ Age Range: {result.MinAge} - {maxAge}\n" +
                                 $"Method: {result.VerificationMethod}";
                ResultLabel.TextColor = Colors.Green;
            }
            else if (result.UserDeclined)
            {
                ResultLabel.Text = "✗ User declined to share age information";
                ResultLabel.TextColor = Colors.Orange;
                
                await DisplayAlertAsync("Declined", result.ErrorMessage ?? "User declined age verification", "OK");
            }
            else
            {
                ResultLabel.Text = $"✗ {result.ErrorMessage}";
                ResultLabel.TextColor = Colors.Red;
                
                await DisplayAlertAsync("Information", result.ErrorMessage ?? "Age verification not available", "OK");
            }
        }
        catch (Exception ex)
        {
            ResultLabel.Text = $"✗ Exception: {ex.Message}";
            ResultLabel.TextColor = Colors.Red;
            
            await DisplayAlertAsync("Error", $"An error occurred: {ex.Message}", "OK");
        }
        finally
        {
            CheckAgeButton.IsEnabled = true;
        }
    }
}
