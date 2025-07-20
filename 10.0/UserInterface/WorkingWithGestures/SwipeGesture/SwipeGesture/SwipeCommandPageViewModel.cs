using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace SwipeGesture
{
    public class SwipeCommandPageViewModel : INotifyPropertyChanged
    {
        private string swipeDirection;
        private int totalSwipes;
        private int upSwipes;
        private int downSwipes;
        private int leftSwipes;
        private int rightSwipes;

        public ICommand SwipeCommand => new Command<string>(Swipe);

        public string SwipeDirection
        {
            get => swipeDirection;
            private set
            {
                swipeDirection = value;
                OnPropertyChanged();
            }
        }

        public int TotalSwipes
        {
            get => totalSwipes;
            private set
            {
                totalSwipes = value;
                OnPropertyChanged();
            }
        }

        public int UpSwipes
        {
            get => upSwipes;
            private set
            {
                upSwipes = value;
                OnPropertyChanged();
            }
        }

        public int DownSwipes
        {
            get => downSwipes;
            private set
            {
                downSwipes = value;
                OnPropertyChanged();
            }
        }

        public int LeftSwipes
        {
            get => leftSwipes;
            private set
            {
                leftSwipes = value;
                OnPropertyChanged();
            }
        }

        public int RightSwipes
        {
            get => rightSwipes;
            private set
            {
                rightSwipes = value;
                OnPropertyChanged();
            }
        }

        public SwipeCommandPageViewModel()
        {
            SwipeDirection = "🎯 Ready to swipe!";
            TotalSwipes = 0;
            UpSwipes = 0;
            DownSwipes = 0;
            LeftSwipes = 0;
            RightSwipes = 0;
        }

        void Swipe(string direction)
        {
            var emoji = GetDirectionEmoji(direction);
            SwipeDirection = $"{emoji} Command executed: {direction}!";
            
            // Update statistics
            TotalSwipes++;
            
            switch (direction.ToLower())
            {
                case "up":
                    UpSwipes++;
                    break;
                case "down":
                    DownSwipes++;
                    break;
                case "left":
                    LeftSwipes++;
                    break;
                case "right":
                    RightSwipes++;
                    break;
            }

            // Add motivational messages
            if (TotalSwipes == 1)
            {
                SwipeDirection += " 🎉 First swipe!";
            }
            else if (TotalSwipes == 10)
            {
                SwipeDirection += " 🏆 10 swipes achieved!";
            }
            else if (TotalSwipes % 25 == 0)
            {
                SwipeDirection += $" 🌟 {TotalSwipes} swipes milestone!";
            }
        }

        private string GetDirectionEmoji(string direction)
        {
            return direction.ToLower() switch
            {
                "up" => "⬆️",
                "down" => "⬇️",
                "left" => "⬅️",
                "right" => "➡️",
                _ => "🎯"
            };
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
