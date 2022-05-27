using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MauiEmbedding.Windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            // Setup .NET MAUI
            var builder = MauiApp.CreateBuilder();

            // Add Microsoft.Maui Controls
            AppHostBuilderExtensions.UseMauiEmbedding<Microsoft.Maui.Controls.Application>(builder);

            var mauiApp = builder.Build();

            // Create and save a Maui Context. This is needed for creating Platform UI
            mauiContext = new MauiContext(mauiApp.Services);
            var mauiControl = new MauiControl().ToPlatform(mauiContext);
            
            myStackPanel.Children.Add(mauiControl);
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";
        }
    }
}
