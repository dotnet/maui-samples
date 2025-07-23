using WindowButtonsHandler.Controls;

namespace WindowButtonsHandler
{
    public partial class MainPage : ContentPage
    {
        bool _isMinimizable = true;
        bool _isMaximizable = true;

        public MainPage()
        {
            InitializeComponent();
        }

        void OnToggleMinimizeClicked(object sender, EventArgs e)
        {
            // Toggle the minimize state
            _isMinimizable = !_isMinimizable;

            // Get the parent window and update its properties
            var window = GetParentWindow();

            if (window is not null)
            {
                // Cast to our custom window type to access IsMinimizable property
                if (window is ICustomWindow customWindow)
                {
                    customWindow.IsMinimizable = _isMinimizable;
                }
            }

            // Update the UI to reflect the current state
            UpdateStatusLabel();
        }

        void OnToggleMaximizeClicked(object sender, EventArgs e)
        {
            // Toggle the maximize state
            _isMaximizable = !_isMaximizable;

            // Get the parent window and update its properties
            var window = GetParentWindow();

            if (window is not null)
            {
                // Cast to our custom window type to access IsMaximizable property
                if (window is ICustomWindow customWindow)
                {
                    customWindow.IsMaximizable = _isMaximizable;
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
        IWindow? GetParentWindow()
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
            StatusLabel.Text = $"Minimize: {(_isMinimizable ? "Enabled" : "Disabled")} | " +   
                $"Maximize: {(_isMaximizable ? "Enabled" : "Disabled")}";
        }
    }
}