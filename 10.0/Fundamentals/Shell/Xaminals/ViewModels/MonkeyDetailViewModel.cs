using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Web;
using Xaminals.Models;

namespace Xaminals.ViewModels
{
    public class MonkeyDetailViewModel : IQueryAttributable, INotifyPropertyChanged
    {
        public Animal Monkey { get; private set; }

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
