﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace BehaviorsDemos
{
    public class HomePageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Person> People { get; set; }

        public ICommand OutputAgeCommand { get; set; }

        public string SelectedItemText { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public HomePageViewModel()
        {
            People = new ObservableCollection<Person>
            {
                new Person ("Steve", 21),
                new Person ("John", 37),
                new Person ("Tom", 42),
                new Person ("Lucas", 29),
                new Person ("Tariq", 39),
                new Person ("Jane", 30)
            };
            OutputAgeCommand = new Command<Person>(OutputAge);
        }

        void OutputAge(Person person)
        {
            SelectedItemText = string.Format("{0} is {1} years old.", person.Name, person.Age);
            OnPropertyChanged("SelectedItemText");
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
