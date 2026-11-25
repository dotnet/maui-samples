using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using ControlGallery.Models;

namespace ControlGallery.ViewModels
{
    public class RefreshViewDemoPageViewModel : INotifyPropertyChanged
    {
        const int RefreshDuration = 2;
        int itemNumber = 1;
        readonly Random random;
        bool isRefreshing;

        // The accent palette from Resources/Styles/Colors.xaml.
        static readonly Color[] palette =
        [
            Color.FromArgb("#7C5CFF"), Color.FromArgb("#3B82F6"), Color.FromArgb("#0284C7"),
            Color.FromArgb("#0D9488"), Color.FromArgb("#16A34A"), Color.FromArgb("#F59E0B"),
            Color.FromArgb("#EA580C"), Color.FromArgb("#F43F5E"), Color.FromArgb("#EC4899")
        ];

        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<RefreshItem> Items { get; private set; }

        public ICommand RefreshCommand => new Command(async () => await RefreshItemsAsync());

        public RefreshViewDemoPageViewModel()
        {
            random = new Random();
            Items = new ObservableCollection<RefreshItem>();
            AddItems();
        }

        void AddItems()
        {
            for (int i = 0; i < 50; i++)
            {
                Items.Add(new RefreshItem
                {
                    Color = palette[random.Next(palette.Length)],
                    Name = $"Item {itemNumber++}"
                });
            }
        }

        async Task RefreshItemsAsync()
        {
            IsRefreshing = true;
            await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
            AddItems();
            IsRefreshing = false;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}

