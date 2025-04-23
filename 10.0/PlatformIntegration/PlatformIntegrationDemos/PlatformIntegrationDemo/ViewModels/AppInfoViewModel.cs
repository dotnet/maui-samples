﻿namespace PlatformIntegrationDemo.ViewModels
{
    public class AppInfoViewModel : BaseViewModel
    {
        public string AppPackageName => AppInfo.PackageName;
        public string AppName => AppInfo.Name;
        public string AppVersion => AppInfo.VersionString;
        public string AppBuild => AppInfo.BuildString;
        public string AppTheme => AppInfo.RequestedTheme.ToString();
        public Command ShowSettingsUICommand { get; }

        public AppInfoViewModel()
        {
            ShowSettingsUICommand = new Command(() => AppInfo.ShowSettingsUI());
        }
    }
}

