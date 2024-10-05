namespace TableViewDemos
{
	public partial class DataIntentPage : ContentPage
	{
		public DataIntentPage ()
		{
			InitializeComponent ();
		}

        void OnViewCellTapped(object sender, EventArgs e)
        {
            target.IsVisible = !target.IsVisible;
            viewCell.ForceUpdateSize();
        }
	}
}
