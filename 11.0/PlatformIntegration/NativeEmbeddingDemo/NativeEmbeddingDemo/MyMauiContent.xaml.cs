namespace NativeEmbeddingDemo
{
    public partial class MyMauiContent : ContentView
    {
        int count = 0;

        public Image DotNetBot => image;

        public MyMauiContent()
        {
            InitializeComponent();
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);

            await image.ScaleToAsync(1.2, 60);
            await image.ScaleToAsync(1, 60);
        }
    }
}
