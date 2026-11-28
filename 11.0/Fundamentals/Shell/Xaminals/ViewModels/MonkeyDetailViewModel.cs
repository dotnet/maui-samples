using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xaminals.Models;

namespace Xaminals.ViewModels
{
    public class MonkeyDetailViewModel : IQueryAttributable, INotifyPropertyChanged
    {
        public Animal Monkey { get; private set; }

        // Bound to the BackButtonBehavior in MonkeyDetailPage.xaml
        public ICommand BackCommand { get; } = new Command(async () => await Shell.Current.GoToAsync(".."));

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Monkey = query["Monkey"] as Animal;
            OnPropertyChanged("Monkey");
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
