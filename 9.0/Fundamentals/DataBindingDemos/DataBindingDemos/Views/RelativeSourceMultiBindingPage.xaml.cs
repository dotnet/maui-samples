namespace DataBindingDemos
{
    public partial class RelativeSourceMultiBindingPage : ContentPage
    {
        public RelativeSourceMultiBindingPage()
        {
            InitializeComponent();
        }

        void OnTapGestureRecognized(object sender, EventArgs e)
        {
            Expander expanderGrid = sender as Expander;
            expanderGrid.IsExpanded = !expanderGrid.IsExpanded;
            expanderGrid.IsVisible = false;
        }
    }
}
