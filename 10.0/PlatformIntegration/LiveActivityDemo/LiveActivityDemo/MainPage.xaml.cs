using System.Runtime.InteropServices;

namespace LiveActivityDemo;

public partial class MainPage : ContentPage
{
	int _tick = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private void StartBtn_Clicked(object sender, EventArgs e)
	{
#if IOS
		bool ok = ActivityKitBridge.Start("#1", "Preparing order…", 0.1);
		StatusTxt.Text = ok ? "Started" : "Start failed";
#else
            StatusTxt.Text = "iOS only";
#endif
	}

	private void UpdateBtn_Clicked(object sender, EventArgs e)
	{
#if IOS
		_tick = Math.Min(10, _tick + 1);
		var progress = _tick / 10.0;
		bool ok = ActivityKitBridge.Update($"Progress: {progress:P0}", progress);
		StatusTxt.Text = ok ? $"Updated to {progress:P0}" : "Update failed";
#else
            StatusTxt.Text = "iOS only";
#endif
	}

	private void EndBtn_Clicked(object sender, EventArgs e)
	{
#if IOS
		bool ok = ActivityKitBridge.End();
		StatusTxt.Text = ok ? "Ended" : "End failed";
		_tick = 0;
#else
            StatusTxt.Text = "iOS only";
#endif
	}
}

internal static class ActivityKitBridge
{
#if IOS
	[DllImport("__Internal", EntryPoint = "LA_Start")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _Start(
		[MarshalAs(UnmanagedType.LPUTF8Str)] string orderId,
		[MarshalAs(UnmanagedType.LPUTF8Str)] string title,
		double progress);

	[DllImport("__Internal", EntryPoint = "LA_Update")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _Update(
		[MarshalAs(UnmanagedType.LPUTF8Str)] string title,
		double progress);

	[DllImport("__Internal", EntryPoint = "LA_End")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _End();

	public static bool Start(string orderId, string title, double progress) => _Start(orderId, title, progress);
	public static bool Update(string title, double progress) => _Update(title, progress);
	public static bool End() => _End();
#else
    public static bool Start(string orderId, string title, double progress) => false;
    public static bool Update(string title, double progress) => false;
    public static bool End() => false;
#endif
}