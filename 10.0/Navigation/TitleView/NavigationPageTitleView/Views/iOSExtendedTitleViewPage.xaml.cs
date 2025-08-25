using System;
using System.Windows.Input;


namespace NavigationPageTitleView
{
    public partial class iOSExtendedTitleViewPage : ContentPage
    {
        ICommand _returnToMenuPage;

        public iOSExtendedTitleViewPage(ICommand restore)
        {
            InitializeComponent();

            _returnToMenuPage = restore;
            // Note: Effects have been replaced with Handlers in .NET MAUI
            // _searchBar.Effects.Add(Effect.Resolve("XamarinDocs.SearchBarEffect"));
        }

        void OnReturnButtonClicked(object sender, EventArgs e)
        {
            _returnToMenuPage.Execute(null);
        }
    }
}
