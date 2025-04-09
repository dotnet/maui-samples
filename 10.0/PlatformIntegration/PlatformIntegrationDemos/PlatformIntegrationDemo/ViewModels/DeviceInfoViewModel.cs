﻿namespace PlatformIntegrationDemo.ViewModels
{
    public class DeviceInfoViewModel : BaseViewModel
    {
        DisplayInfo screenMetrics;

        public string Model => DeviceInfo.Model;

        public string Manufacturer => DeviceInfo.Manufacturer;

        public string Name => DeviceInfo.Name;

        public string VersionString => DeviceInfo.VersionString;

        public string Version => DeviceInfo.Version.ToString();

        public DevicePlatform Platform => DeviceInfo.Platform;

        public DeviceIdiom Idiom => DeviceInfo.Idiom;

        public DeviceType DeviceType => DeviceInfo.DeviceType;

        public double Rotation =>
            ScreenMetrics.Rotation switch
            {
                DisplayRotation.Rotation0 => 0,
                DisplayRotation.Rotation90 => -90,
                DisplayRotation.Rotation180 => -180,
                DisplayRotation.Rotation270 => -270,
                _ => 0
            };

        public DisplayInfo ScreenMetrics
        {
            get => screenMetrics;
            set => SetProperty(ref screenMetrics, value, onChanged: () => OnPropertyChanged(nameof(Rotation)));
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            DeviceDisplay.MainDisplayInfoChanged += OnScreenMetricsChanged;
            ScreenMetrics = DeviceDisplay.MainDisplayInfo;
        }

        public override void OnDisappearing()
        {
            DeviceDisplay.MainDisplayInfoChanged -= OnScreenMetricsChanged;

            base.OnDisappearing();
        }

        void OnScreenMetricsChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            ScreenMetrics = e.DisplayInfo;
        }
    }
}

