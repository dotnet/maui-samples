﻿using System.Windows.Input;

namespace PlatformIntegrationDemo.ViewModels
{
    public class KeepScreenOnViewModel : BaseViewModel
    {
        public KeepScreenOnViewModel()
        {
            RequestActiveCommand = new Command(OnRequestActive);
            RequestReleaseCommand = new Command(OnRequestRelease);
        }

        public bool IsActive => DeviceDisplay.KeepScreenOn;

        public ICommand RequestActiveCommand { get; }

        public ICommand RequestReleaseCommand { get; }

        public override void OnDisappearing()
        {
            OnRequestRelease();

            base.OnDisappearing();
        }

        void OnRequestActive()
        {
            DeviceDisplay.KeepScreenOn = true;

            OnPropertyChanged(nameof(IsActive));
        }

        void OnRequestRelease()
        {
            DeviceDisplay.KeepScreenOn = false;

            OnPropertyChanged(nameof(IsActive));
        }
    }
}

