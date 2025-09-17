using WindowButtonsHandler.Controls;
using WindowButtonsHandler.Extensions;

namespace WindowButtonsHandler.Handlers
{
    /// <summary>
    /// Mac Catalyst-specific implementation of the CustomWindowHandler.
    /// This partial class contains the platform-specific logic for managing
    /// window button visibility on macOS through Mac Catalyst.
    /// </summary>
    public partial class CustomWindowHandler
    {
        /// <summary>
        /// Handles changes to the CanMinimize property by updating the Mac Catalyst window.
        /// This method is called automatically when the CanMinimize property changes.
        /// </summary>
        /// <param name="handler">The handler instance managing the window</param>
        /// <param name="window">The custom window instance with the changed property</param>
        public static partial void MapCanMinimize(CustomWindowHandler handler, ICustomWindow window)
        {
            // Ensure we have a valid UIWindow
            if (handler.PlatformView is not null)
            {
                try
                {
                    handler.PlatformView.UpdateCanMinimize(window);
                }
                catch (Exception ex)
                {
                    // Log any errors that occur during the update
                    System.Diagnostics.Debug.WriteLine($"Error updating minimize button: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Handles changes to the CanMaximize property by updating the Mac Catalyst window.
        /// This method is called automatically when the CanMaximize property changes.
        /// </summary>
        /// <param name="handler">The handler instance managing the window</param>
        /// <param name="window">The custom window instance with the changed property</param>
        public static partial void MapCanMaximize(CustomWindowHandler handler, ICustomWindow window)
        {
            // Ensure we have a valid UIWindow
            if (handler.PlatformView is not null)
            {
                try
                {
                    handler.PlatformView.UpdateCanMaximize(window);
                }
                catch (Exception ex)
                {
                    // Log any errors that occur during the update
                    System.Diagnostics.Debug.WriteLine($"Error updating maximize button: {ex.Message}");
                }
            }
        }
    }
}