using WindowButtonsHandler.Controls;

namespace WindowButtonsHandler
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new CustomWindow(new AppShell())
            {
                Title = "Window Buttons Sample"
            };

            // Set initial window properties
            window.IsMinimizable = true;
            window.IsMaximizable = true;

            return window;
        }
    }
}