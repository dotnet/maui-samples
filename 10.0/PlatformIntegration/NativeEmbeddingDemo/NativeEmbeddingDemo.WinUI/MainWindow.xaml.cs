using Microsoft.Maui.Controls.Embedding;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NativeEmbeddingDemo.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Microsoft.UI.Xaml.Window
    {
        public static class MyEmbeddedMauiApp
        {
            static MauiApp? _shared;

            public static MauiApp Shared =>
                _shared ??= MauiProgram.CreateMauiApp();
        }

        Microsoft.UI.Xaml.Window? _window;
        IMauiContext? _windowContext;
        MyMauiContent? _mauiView;
        FrameworkElement? _nativeView;

        public IMauiContext WindowContext =>
            _windowContext ??= MyEmbeddedMauiApp.Shared.CreateEmbeddedWindowContext(_window ?? throw new InvalidOperationException());

        public MainWindow()
        {
            this.InitializeComponent();
            _window ??= this;
        }

        private async void OnRootLayoutLoaded(object? sender, RoutedEventArgs e)
        {
            // Handle the event firing multiple times
            if (_nativeView is not null)
                return;

            await Task.Yield();

            // Create WinUI button
            var nativeButton = new Microsoft.UI.Xaml.Controls.Button();
            nativeButton.Content = "WinUI button above .NET MAUI controls";
            nativeButton.HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center;
            nativeButton.Click += OnWindowsButtonClicked;
            rootLayout.Children.Add(nativeButton);

            // App context
            //    // Ensure .NET MAUI app is built before creating .NET MAUI views
            //    var mauiApp = MauiProgram.CreateMauiApp();

            //    // Create .NET MAUI context
            //    var mauiContext = new MauiContext(mauiApp.Services);

            //    // Create .NET MAUI content
            //    _mauiView = new MyMauiContent();

            //    // Create native view
            //    _nativeView = _mauiView.ToPlatformEmbedded(mauiContext);

            // Window context
            // Create MAUI embedded window context
            var context = WindowContext;

            // Create .NET MAUI content
            _mauiView = new MyMauiContent();

            // Create native view
            _nativeView = _mauiView.ToPlatformEmbedded(context);

            // Add native view to layout
            rootLayout.Children.Add(_nativeView);
        }

        private void OnWindowClosed(object? sender, WindowEventArgs e)
        {
            // Remove the view from the UI
            rootLayout.Children.Remove(_nativeView);

            // Cleanup any Window
            if (_mauiView?.Window is IWindow window)
                window.Destroying();
        }

        private async void OnWindowsButtonClicked(object? sender, RoutedEventArgs e)
        {
            if (_mauiView?.DotNetBot is not Microsoft.Maui.Controls.Image bot)
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
