using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XamlSamples
{
    public class ClockViewModel : INotifyPropertyChanged
    {
        DateTime _dateTime;
        Timer _timer;
        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime DateTime
        {
            get => _dateTime;
            set
            {
                if (_dateTime != value)
                {
                    _dateTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public ClockViewModel()
        {
            this.DateTime = DateTime.Now;
            _timer = new Timer(new TimerCallback((s) => this.DateTime = DateTime.Now), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        ~ClockViewModel() => _timer.Dispose();

        public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
