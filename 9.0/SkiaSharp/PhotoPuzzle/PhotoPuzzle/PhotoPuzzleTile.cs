namespace PhotoPuzzle
{
    class PhotoPuzzleTile : ContentView
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public PhotoPuzzleTile(int row, int col, ImageSource imageSource)
        {
            Row = row;
            Col = col;

            Padding = new Thickness(1);
            Content = new Image
            {
                Source = imageSource
            };
        }
    }
}

