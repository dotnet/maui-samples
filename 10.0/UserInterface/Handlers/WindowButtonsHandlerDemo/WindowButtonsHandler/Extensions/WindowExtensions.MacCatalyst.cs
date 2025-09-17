using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
using UIKit;
using WindowButtonsHandler.Controls;

namespace WindowButtonsHandler.Extensions
{
    /// <summary>
    /// Extension methods for UIWindow that provide Mac Catalyst-specific window management functionality.
    /// These methods use Objective-C runtime calls to access native macOS window features
    /// that aren't directly exposed through the UIKit API.
    /// </summary>
    public static class WindowExtensions
    {
        /// <summary>
        /// Path to the Objective-C runtime library used for direct method calls.
        /// </summary>
        const string LibobjcDylib = "/usr/lib/libobjc.dylib";

        /// <summary>
        /// P/Invoke declaration for sending boolean messages to Objective-C objects.
        /// Used to hide/show window buttons by calling setHidden: on NSButton objects.
        /// </summary>
        [DllImport(LibobjcDylib, EntryPoint = "objc_msgSend")]
        static extern void void_objc_msgSend_bool(IntPtr receiver, IntPtr selector, bool arg1);

        /// <summary>
        /// P/Invoke declaration for sending pointer messages to Objective-C objects.
        /// Used to retrieve NSButton objects from NSWindow using standardWindowButton:.
        /// </summary>
        [DllImport(LibobjcDylib, EntryPoint = "objc_msgSend")]
        internal static extern IntPtr IntPtr_objc_msgSend_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1);

        /// <summary>
        /// Cached selector for the standardWindowButton: method.
        /// </summary>
        static Selector? StandardWindowButtonSelector;
        static Selector StandardWindowButton => StandardWindowButtonSelector ??= new Selector("standardWindowButton:");

        /// <summary>
        /// Cached selector for the setHidden: method.
        /// </summary>
        static Selector? StandardWindowButtonSetHiddenSelector;
        static Selector StandardWindowButtonSetHidden => StandardWindowButtonSetHiddenSelector ??= new Selector("setHidden:");

        /// <summary>
        /// Enumeration of standard NSWindow button types.
        /// </summary>
        enum NSWindowButton : ulong
        {
            CloseButton,
            MinimizeButton,
            MaximizeButton
        }

        /// <summary>
        /// Updates the visibility of the minimize button on a Mac Catalyst window.
        /// </summary>
        /// <param name="platformWindow">The UIWindow to update</param>
        /// <param name="window">The MAUI window containing the minimize state</param>
        public static void UpdateCanMinimize(this UIWindow platformWindow, IWindow window)
        {
            // Get the underlying NSWindow from the UIWindow
            var nsWindow = GetNSWindowFromUIWindow(platformWindow);

            if (nsWindow != null)
            {
                // Retrieve the minimize button from the NSWindow
                var minimizeButton = Runtime.GetNSObject(
                    IntPtr_objc_msgSend_IntPtr(
                        nsWindow.Handle,
                        StandardWindowButton.Handle,
                        (IntPtr)(ulong)NSWindowButton.MinimizeButton));

                if (minimizeButton is not null)
                {
                    // Determine if the button should be visible
                    bool canMinimize = window is not ICustomWindow customWindow || customWindow.CanMinimize;

                    // Hide or show the button
                    void_objc_msgSend_bool(
                        minimizeButton.Handle,
                        StandardWindowButtonSetHidden.Handle,
                        !canMinimize);
                }
                else
                {
                    Console.WriteLine("Minimize button not found.");
                }
            }
            else
            {
                Console.WriteLine("NSWindow not found for UIWindow.");
            }
        }

        /// <summary>
        /// Updates the visibility of the maximize button on a Mac Catalyst window.
        /// </summary>
        /// <param name="platformWindow">The UIWindow to update</param>
        /// <param name="window">The MAUI window containing the maximize state</param>
        public static void UpdateCanMaximize(this UIWindow platformWindow, IWindow window)
        {
            // Get the underlying NSWindow from the UIWindow
            var nsWindow = GetNSWindowFromUIWindow(platformWindow);

            if (nsWindow != null)
            {
                // Retrieve the maximize button from the NSWindow
                var maximizeButton = Runtime.GetNSObject(
                    IntPtr_objc_msgSend_IntPtr(
                        nsWindow.Handle,
                        StandardWindowButton.Handle,
                        (IntPtr)(ulong)NSWindowButton.MaximizeButton));

                if (maximizeButton is not null)
                {
                    // Determine if the button should be visible
                    bool canMaximize = window is not ICustomWindow customWindow || customWindow.CanMaximize;

                    // Hide or show the button
                    void_objc_msgSend_bool(
                        maximizeButton.Handle,
                        StandardWindowButtonSetHidden.Handle,
                        !canMaximize);
                }
                else
                {
                    Console.WriteLine("Maximize button not found.");
                }
            }
            else
            {
                Console.WriteLine("NSWindow not found for UIWindow.");
            }
        }

        /// <summary>
        /// Retrieves the NSWindow associated with a UIWindow in Mac Catalyst.
        /// </summary>
        /// <param name="window">The UIWindow to get the NSWindow for</param>
        /// <returns>The associated NSWindow, or null if not found</returns>
        static NSObject? GetNSWindowFromUIWindow(UIWindow window)
        {
            if (window is null || window.Handle == IntPtr.Zero)
            {
                Console.WriteLine("Invalid UIWindow provided.");
                return null;
            }

            var nsApplication = Runtime.GetNSObject(Class.GetHandle("NSApplication"));
            if (nsApplication is null)
            {
                Console.WriteLine("NSApplication not found.");
                return null;
            }

            var sharedApplication = nsApplication.PerformSelector(new Selector("sharedApplication"));
            if (sharedApplication is null)
            {
                Console.WriteLine("Shared NSApplication not found.");
                return null;
            }

            var applicationDelegate = sharedApplication.PerformSelector(new Selector("delegate"));
            if (applicationDelegate is null)
            {
                Console.WriteLine("Application delegate not found.");
                return null;
            }

            return GetNSWindow(window, applicationDelegate, maxRetries: 1);
        }

        /// <summary>
        /// Attempts to retrieve the NSWindow for a UIWindow with a limited number of retries.
        /// </summary>
        /// <param name="window">The UIWindow to get the NSWindow for</param>
        /// <param name="applicationDelegate">The application delegate that manages the window bridge</param>
        /// <param name="maxRetries">Maximum number of retries</param>
        /// <param name="retryCount">Current retry attempt</param>
        /// <returns>The associated NSWindow, or null if not found after retries</returns>
        static NSObject? GetNSWindow(UIWindow window, NSObject applicationDelegate, int maxRetries, int retryCount = 0)
        {
            if (applicationDelegate is null || applicationDelegate.Handle == IntPtr.Zero)
            {
                Console.WriteLine("Invalid application delegate.");
                return null;
            }
            
            // Use the custom hostWindowForUIWindow: selector
            var nsWindowHandle = IntPtr_objc_msgSend_IntPtr(applicationDelegate.Handle,
                Selector.GetHandle("hostWindowForUIWindow:"), window.Handle);
            var nsWindow = Runtime.GetNSObject<NSObject>(nsWindowHandle);

            if (nsWindow is null && retryCount < maxRetries)
            {
                Console.WriteLine($"Retry {retryCount + 1}/{maxRetries}: NSWindow not found for UIWindow.Handle={window.Handle}.");
                Thread.Sleep(500);
                return GetNSWindow(window, applicationDelegate, maxRetries, retryCount + 1);
            }

            if (nsWindow is null)
            {
                Console.WriteLine($"Failed to get NSWindow after {maxRetries} retries.");
            }

            return nsWindow;
        }
    }
}