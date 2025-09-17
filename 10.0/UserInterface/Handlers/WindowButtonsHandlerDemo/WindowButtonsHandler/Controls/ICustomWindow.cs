namespace WindowButtonsHandler.Controls
{
    /// <summary>
    /// Extended interface for windows that support custom button visibility control.
    /// This interface adds properties for controlling minimize and maximize button visibility
    /// beyond what the standard IWindow interface provides.
    /// </summary>
    public interface ICustomWindow : IWindow
    {
        /// <summary>
        /// Gets or sets whether the window's minimize button should be visible and functional.
        /// On Mac Catalyst, this controls the yellow minimize button in the title bar.
        /// </summary>
        bool CanMinimize { get; set; }

        /// <summary>
        /// Gets or sets whether the window's maximize button should be visible and functional.
        /// On Mac Catalyst, this controls the green maximize/zoom button in the title bar.
        /// </summary>
        bool CanMaximize { get; set; }
    }
}
