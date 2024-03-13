using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NativeEmbeddingDemo.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Microsoft.UI.Xaml.Window
    {
        public static readonly Lazy<MauiApp> MauiApp = new(() =>
        {
            var mauiApp = MauiProgram.CreateMauiApp(builder =>
            {
                builder.UseMauiEmbedding();
            });
            return mauiApp;
        });

        public static bool UseWindowContext = true;

        MyMauiContent? mauiView;

        public MainWindow()
        {
            this.InitializeComponent();

            // Add a StackPanel to the root layout
            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
                VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Stretch,
                Spacing = 8,
                Padding = new Microsoft.UI.Xaml.Thickness(20)
            };
            rootLayout.Children.Add(stackPanel);

            // Create WinUI button
            var nativeButton = new Microsoft.UI.Xaml.Controls.Button();
            nativeButton.Content = "WinUI button above .NET MAUI controls";
            nativeButton.HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center;
            nativeButton.Click += OnWindowsButtonClicked;
            stackPanel.Children.Add(nativeButton);

            // Ensure .NET MAUI app is built before creating .NET MAUI views
            var mauiApp = MainWindow.MauiApp.Value;

            // Create .NET MAUI context
            var mauiContext = UseWindowContext
                ? mauiApp.CreateEmbeddedWindowContext(this) // Create window context
                : new MauiContext(mauiApp.Services);        // Create app context

            // Create .NET MAUI content
            mauiView = new MyMauiContent();

            // Create native view
            var nativeView = mauiView.ToPlatformEmbedded(mauiContext);

            // Add native view to layout
            stackPanel.Children.Add(nativeView);
        }

        private async void OnWindowsButtonClicked(object? sender, RoutedEventArgs e)
        {
            if (mauiView?.DotNetBot is not Microsoft.Maui.Controls.Image bot)
                return;

            await bot.RotateTo(360, 1000);
            bot.Rotation = 0;

            bot.HeightRequest = 90;
        }

        private void OnNewWindowClicked(object sender, RoutedEventArgs e)
        {
            var window = new MainWindow();
            window.Activate();
        }

    }
}
