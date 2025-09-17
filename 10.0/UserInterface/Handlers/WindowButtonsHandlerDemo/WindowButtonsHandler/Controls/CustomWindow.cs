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
        /// Backing field for the CanMinimize property
        /// </summary>
        bool _canMinimize = true;

        /// <summary>
        /// Backing field for the CanMaximize property
        /// </summary>
        bool _canMaximize = true;

        /// <summary>
        /// Gets or sets whether the window's minimize button should be visible and functional.
        /// When set to false, the minimize button will be hidden on supported platforms.
        /// </summary>
        public bool CanMinimize
        {
            get => _canMinimize;
            set
            {
                if (_canMinimize != value)
                {
                    _canMinimize = value;
                    // Notify the handler that this property has changed
                    Handler?.UpdateValue(nameof(CanMinimize));
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the window's maximize button should be visible and functional.
        /// When set to false, the maximize button will be hidden on supported platforms.
        /// </summary>
        public bool CanMaximize
        {
            get => _canMaximize;
            set
            {
                if (_canMaximize != value)
                {
                    _canMaximize = value;
                    // Notify the handler that this property has changed
                    Handler?.UpdateValue(nameof(CanMaximize));
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