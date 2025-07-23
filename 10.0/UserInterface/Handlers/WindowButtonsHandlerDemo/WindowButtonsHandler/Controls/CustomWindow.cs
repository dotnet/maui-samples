namespace WindowButtonsHandler.Controls
{
    /// <summary>
    /// Custom window implementation that extends the standard MAUI Window class
    /// with additional properties for controlling window button visibility.
    /// This class is used in conjunction with CustomWindowHandler to provide
    /// platform-specific window management functionality.
    /// </summary>
    public class CustomWindow : Window, ICustomWindow
    {
        /// <summary>
        /// Backing field for the IsMinimizable property
        /// </summary>
        private bool _isMinimizable = true;

        /// <summary>
        /// Backing field for the IsMaximizable property
        /// </summary>
        private bool _isMaximizable = true;

        /// <summary>
        /// Gets or sets whether the window's minimize button should be visible and functional.
        /// When set to false, the minimize button will be hidden on supported platforms.
        /// </summary>
        public bool IsMinimizable
        {
            get => _isMinimizable;
            set
            {
                if (_isMinimizable != value)
                {
                    _isMinimizable = value;
                    // Notify the handler that this property has changed
                    Handler?.UpdateValue(nameof(IsMinimizable));
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the window's maximize button should be visible and functional.
        /// When set to false, the maximize button will be hidden on supported platforms.
        /// </summary>
        public bool IsMaximizable
        {
            get => _isMaximizable;
            set
            {
                if (_isMaximizable != value)
                {
                    _isMaximizable = value;
                    // Notify the handler that this property has changed
                    Handler?.UpdateValue(nameof(IsMaximizable));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the CustomWindow class.
        /// </summary>
        public CustomWindow() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CustomWindow class with the specified page.
        /// </summary>
        /// <param name="page">The page to display in the window</param>
        public CustomWindow(Page page) : base(page)
        {
        }
    }
}