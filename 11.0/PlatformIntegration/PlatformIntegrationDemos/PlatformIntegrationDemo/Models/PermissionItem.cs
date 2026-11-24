using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using PlatformIntegrationDemo.ViewModels;

namespace PlatformIntegrationDemo.Models
{
    public class PermissionItem : ObservableObject
    {
        public PermissionItem(string title, Permissions.BasePermission permission)
        {
            Title = title;
            Permission = permission;
            Status = PermissionStatus.Unknown;
        }

        public string Title { get; set; }

        public string Rationale { get; set; }

        public PermissionStatus Status { get; set; }

        public Permissions.BasePermission Permission { get; set; }

        public ICommand CheckStatusCommand =>
            new Command(async () =>
            {
                try
                {
                    Status = await Permission.CheckStatusAsync();
                    OnPropertyChanged(nameof(Status));
                }
                catch (Exception ex)
                {
                    WeakReferenceMessenger.Default.Send(ex, nameof(PermissionException));
                }
            });

        public ICommand RequestCommand =>
            new Command(async () =>
            {
                try
                {
                    Status = await Permission.RequestAsync();
                    OnPropertyChanged(nameof(Status));
                }
                catch (Exception ex)
                {
                    WeakReferenceMessenger.Default.Send(ex, nameof(PermissionException));
                }
            });

        public ICommand ShouldShowRationaleCommand =>
            new Command(() =>
            {
                try
                {
                    Rationale = $"Should show rationale: {Permission.ShouldShowRationale()}";
                    OnPropertyChanged(nameof(Rationale));
                }
                catch (Exception ex)
                {
                    WeakReferenceMessenger.Default.Send(ex, nameof(PermissionException));
                }
            });
    }
}

