namespace CarouselViewDemos.Views
{
    public partial class DynamicSizeItemsPage : ContentPage
    {
        public DynamicSizeItemsPage()
        {
            InitializeComponent();
        }

        void OnImageTapped(object sender, EventArgs e)
        {
            Image image = sender as Image;
            image.HeightRequest = image.WidthRequest = image.HeightRequest.Equals(150) ? 200 : 150;
            Border border = ((Border)image.Parent.Parent);
            border.HeightRequest = border.HeightRequest.Equals(360) ? 410 : 360;
        }
    }
}
