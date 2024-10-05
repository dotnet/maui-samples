namespace GameOfLife
{
    class GameCell : BoxView
    {
        public int Row { get; set; }
        public int Col { get; set; }

        bool isAlive;
        public bool IsAlive
        {
            get
            {
                return isAlive;
            }
            set
            {
                if (isAlive != value)
                {
                    isAlive = value;
                    BackgroundColor = isAlive ? Colors.Black : Colors.White;
                }
            }
        }

        public event EventHandler Tapped;

        public GameCell()
        {
            BackgroundColor = Colors.White;

            TapGestureRecognizer tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (sender, args) =>
            {
                Tapped?.Invoke(this, EventArgs.Empty);
            };
            GestureRecognizers.Add(tapGesture);
        }
    }
}
