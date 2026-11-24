using WindowButtonsHandler.Controls;

namespace WindowButtonsHandler
{
    public partial class MainPage : ContentPage
    {
        bool _canMinimize = true;
        bool _canMaximize = true;

        public MainPage()
        {
            InitializeComponent();
        }

        void OnToggleMinimizeClicked(object sender, EventArgs e)
        {
            // Toggle the minimize state
            _canMinimize = !_canMinimize;

            // Get the parent window and update its properties
            var window = GetParentWindow();

            if (window is not null)
            {
                // Cast to our custom window type to access CanMinimize property
                if (window is ICustomWindow customWindow)
                {
                    customWindow.CanMinimize = _canMinimize;
                }
            }

            // Update the UI to reflect the current state
            UpdateStatusLabel();
        }

        void OnToggleMaximizeClicked(object sender, EventArgs e)
        {
            // Toggle the maximize state
            _canMaximize = !_canMaximize;

            // Get the parent window and update its properties
            var window = FindParentWindow();

            if (window is not null)
            {
                // Cast to our custom window type to access CanMaximize property
                if (window is ICustomWindow customWindow)
                {
                    customWindow.CanMaximize = _canMaximize;
                }
            }

            // Update the UI to reflect the current state
            UpdateStatusLabel();
        }

        /// <summary>
        /// Traverses the element hierarchy to find the parent window.
        /// This is necessary because ContentPage doesn't directly expose the window.
        /// </summary>
        /// <returns>The parent IWindow, or null if not found</returns>
        IWindow? FindParentWindow()
        {
            var parent = this.Parent;

            // Walk up the element tree looking for a Window
            while (parent != null)
            {
                if (parent is IWindow window)
                    return window;

                if (parent is Element element)
                    parent = element.Parent;
                else
                    break;
            }

            // Fallback to the first window in the application
            return Application.Current?.Windows?.FirstOrDefault();
        }

        void UpdateStatusLabel()
        {
            StatusLabel.Text = $"Minimize: {(_canMinimize ? "Enabled" : "Disabled")} | " +   
                $"Maximize: {(_canMaximize ? "Enabled" : "Disabled")}";
        }
    }
}