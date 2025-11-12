using Microsoft.Maui.Platform;

namespace Maui.Controls.Sample;

public partial class Example2RespectAll : ContentPage
{
	// Set to true to show keyboard for second screenshot
	private bool _showKeyboard = true;

	public Example2RespectAll()
	{
		InitializeComponent();
		
		Loaded += OnPageLoaded;
	}

	private async void OnPageLoaded(object? sender, EventArgs e)
	{
		// Wait for page to fully render
		await Task.Delay(1500);
		
		if (_showKeyboard)
		{
			Console.WriteLine("========== SHOWING KEYBOARD ==========");
			
			// Focus the bottom entry to show keyboard avoidance
			PhoneEntry.Focus();
			
#if ANDROID || IOS
			// Wait a moment then force show keyboard
			await Task.Delay(300);
			
			// Force show keyboard if focus didn't trigger it
			if (PhoneEntry.Handler?.PlatformView is not null)
			{
#if IOS
				if (PhoneEntry.Handler.PlatformView is UIKit.UITextField textField)
				{
					textField.BecomeFirstResponder();
					Console.WriteLine("BecomeFirstResponder called on PhoneEntry");
				}
#elif ANDROID
				if (PhoneEntry.Handler.PlatformView is Android.Views.View view)
				{
					view.RequestFocus();
					var inputMethodManager = (Android.Views.InputMethods.InputMethodManager?)view.Context?.GetSystemService(Android.Content.Context.InputMethodService);
					inputMethodManager?.ShowSoftInput(view, Android.Views.InputMethods.ShowFlags.Implicit);
					Console.WriteLine("ShowSoftInput called on PhoneEntry");
				}
#endif
			}
			
			// Wait longer for keyboard animation to complete
			await Task.Delay(1500);
			Console.WriteLine("Keyboard should now be visible");
#endif
		}
	}
}
