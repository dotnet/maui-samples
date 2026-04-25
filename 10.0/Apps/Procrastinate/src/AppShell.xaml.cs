using procrastinate.Pages;
using procrastinate.Services;

namespace procrastinate;

public partial class AppShell : Shell
{
	private string? _previousTab;

	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
		
		// Configure platform-specific navigation
		ConfigurePlatformNavigation();
		
		// Close settings page when tab changes
		Navigated += OnShellNavigated;
	}

	private void ConfigurePlatformNavigation()
	{
#if MACCATALYST
		// Mac Catalyst: Use Flyout sidebar, hide TabBar
		MobileTabBar.IsVisible = false;
		DesktopFlyout.IsVisible = true;
		FlyoutBehavior = FlyoutBehavior.Flyout;
#else
		// iOS/Android: Use bottom TabBar, hide Flyout
		MobileTabBar.IsVisible = true;
		DesktopFlyout.IsVisible = false;
		FlyoutBehavior = FlyoutBehavior.Disabled;
#endif
	}

	private async void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
	{
		var currentLocation = e.Current?.Location?.OriginalString;
		
		// Only handle root tab navigation (starts with //)
		if (currentLocation?.StartsWith("//") != true)
		{
			return;
		}
		
		// Extract the tab name (e.g., "//TasksPage" -> "TasksPage")
		var currentTab = currentLocation.TrimStart('/').Split('/')[0];
		
		// Refresh strings to recompute zalgo randomness on navigation
		AppStrings.Refresh();
		
		// If we're on the same tab but navigated (e.g., pushed settings then came back), 
		// don't pop - we might have just opened settings
		if (_previousTab == currentTab)
		{
			_previousTab = currentTab;
			return;
		}
		
		// Tab changed - pop any pages on the navigation stack
		_previousTab = currentTab;
		
		try
		{
			var nav = Current?.CurrentPage?.Navigation;
			if (nav?.NavigationStack?.Count > 1)
			{
				await nav.PopToRootAsync(false);
			}
		}
		catch
		{
			// Ignore navigation errors
		}
	}
}
