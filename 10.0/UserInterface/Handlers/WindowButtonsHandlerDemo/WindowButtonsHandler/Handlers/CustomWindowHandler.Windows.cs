using WindowButtonsHandler.Controls;

namespace WindowButtonsHandler.Handlers
{
    /// <summary>
    /// Windows-specific implementation of the CustomWindowHandler.
    /// This partial class contains the platform-specific logic for managing
    /// window button visibility on Windows platforms.
    /// </summary>
    public partial class CustomWindowHandler
    {
        /// <summary>
        /// Handles changes to the IsMinimizable property by updating the Windows window.
        /// This method is called automatically when the IsMinimizable property changes.
        /// </summary>
        /// <param name="handler">The handler instance managing the window</param>
        /// <param name="window">The custom window instance with the changed property</param>
        public static partial void MapIsMinimizable(CustomWindowHandler handler, ICustomWindow window)
        {
            // TODO: Implement Windows-specific logic for controlling minimize button visibility
            throw new NotImplementedException($"Windows: MapIsMinimizable called with IsMinimizable = {window.IsMinimizable}");
        }

        /// <summary>
        /// Handles changes to the IsMaximizable property by updating the Windows window.
        /// This method is called automatically when the IsMaximizable property changes.
        /// </summary>
        /// <param name="handler">The handler instance managing the window</param>
        /// <param name="window">The custom window instance with the changed property</param>
        public static partial void MapIsMaximizable(CustomWindowHandler handler, ICustomWindow window)
        {
            // TODO: Implement Windows-specific logic for controlling maximize button visibility           
            throw new NotImplementedException($"Windows: MapIsMaximizable called with IsMaximizable = {window.IsMaximizable}");
        }
    }
}