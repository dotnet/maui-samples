using WindowButtonsHandler.Controls;
using WinRT.Interop;

namespace WindowButtonsHandler.Extensions
{
    /// <summary>
    /// Extension methods for Windows platform windows that provide Windows-specific window management functionality.
    /// These methods use WinUI and Microsoft.UI.Windowing APIs to control window button visibility.
    /// </summary>
    public static class WindowExtensions
    {
        /// <summary>
        /// Updates the visibility of the minimize button on a Windows window.
        /// </summary>
        /// <param name="platformWindow">The platform window (Microsoft.UI.Xaml.Window) to update</param>
        /// <param name="window">The MAUI window containing the minimize state</param>
        public static void UpdateCanMinimize(this object platformWindow, IWindow window)
        {
            if (platformWindow is Microsoft.UI.Xaml.Window winUIWindow)
            {
                try
                {
                    // Determine if the button should be visible
                    bool canMinimize = window is not ICustomWindow customWindow || customWindow.CanMinimize;

                    // We need to modify the window style to hide/show the minimize button
                    UpdateWindowStyle(winUIWindow, canMinimize, isMaximize: null);
                }
                catch (Exception ex)
                {
                    // Log any errors that occur during the update
                    System.Diagnostics.Debug.WriteLine($"Error updating minimize button: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Updates the visibility of the maximize button on a Windows window.
        /// </summary>
        /// <param name="platformWindow">The platform window (Microsoft.UI.Xaml.Window) to update</param>
        /// <param name="window">The MAUI window containing the maximize state</param>
        public static void UpdateCanMaximize(this object platformWindow, IWindow window)
        {
            if (platformWindow is Microsoft.UI.Xaml.Window winUIWindow)
            {
                try
                {
                    // Determine if the button should be visible
                    bool canMaximize = window is not ICustomWindow customWindow || customWindow.CanMaximize;

                    // We need to modify the window style to hide/show the maximize button
                    UpdateWindowStyle(winUIWindow, isMinimize: null, canMaximize);
                }
                catch (Exception ex)
                {
                    // Log any errors that occur during the update
                    System.Diagnostics.Debug.WriteLine($"Error updating maximize button: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Updates the window style to control button visibility using Win32 APIs.
        /// </summary>
        /// <param name="window">The WinUI window</param>
        /// <param name="isMinimize">Whether minimize button should be visible (null = no change)</param>
        /// <param name="isMaximize">Whether maximize button should be visible (null = no change)</param>
        private static void UpdateWindowStyle(Microsoft.UI.Xaml.Window window, bool? isMinimize, bool? isMaximize)
        {
            var windowHandle = WindowNative.GetWindowHandle(window);

            // Get current window style
            var currentStyle = GetWindowLong(windowHandle, GWL_STYLE);
            var newStyle = currentStyle;

            // Update minimize button visibility
            if (isMinimize.HasValue)
            {
                if (isMinimize.Value)
                {
                    newStyle |= WS_MINIMIZEBOX;
                }
                else
                {
                    newStyle &= ~WS_MINIMIZEBOX;
                }
            }

            // Update maximize button visibility
            if (isMaximize.HasValue)
            {
                if (isMaximize.Value)
                {
                    newStyle |= WS_MAXIMIZEBOX;
                }
                else
                {
                    newStyle &= ~WS_MAXIMIZEBOX;
                }
            }

            // Apply the new style if it changed
            if (newStyle != currentStyle)
            {
                SetWindowLong(windowHandle, GWL_STYLE, newStyle);

                // Force the window to redraw its frame
                SetWindowPos(windowHandle, IntPtr.Zero, 0, 0, 0, 0,
                    SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
            }
        }

        #region Win32 API Constants and Imports

        private const int GWL_STYLE = -16;
        private const int WS_MINIMIZEBOX = 0x00020000;
        private const int WS_MAXIMIZEBOX = 0x00010000;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_FRAMECHANGED = 0x0020;

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        #endregion
    }
}
