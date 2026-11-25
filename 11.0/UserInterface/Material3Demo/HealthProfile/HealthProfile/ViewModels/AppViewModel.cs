using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HealthProfile.ViewModels;

public class AppViewModel : INotifyPropertyChanged
{
    int _stepGoal = 10000;
    int _currentSteps = 6125;
    string _healthGoal = "Weight Loss";
    bool _isVegan;
    bool _isGlutenFree = true;
    bool _isDairyFree;
    bool _notificationsEnabled = true;

    public int StepGoal
    {
        get => _stepGoal;
        set
        {
            var sanitizedGoal = Math.Clamp(value, 1000, 10000);
            if (_stepGoal != sanitizedGoal)
            {
                _stepGoal = sanitizedGoal;
                OnPropertyChanged();
                NotifyDerivedMetricsChanged();
            }
        }
    }

    public int CurrentSteps
    {
        get => _currentSteps;
        set
        {
            if (_currentSteps != value)
            {
                _currentSteps = value;
                OnPropertyChanged();
                NotifyDerivedMetricsChanged();
            }
        }
    }

    public string HealthGoal
    {
        get => _healthGoal;
        set
        {
            if (_healthGoal != value)
            {
                _healthGoal = value;
                OnPropertyChanged();
                NotifyDerivedMetricsChanged();
            }
        }
    }

    public bool IsVegan
    {
        get => _isVegan;
        set
        {
            if (_isVegan != value)
            {
                _isVegan = value;
                OnPropertyChanged();
                NotifyDerivedMetricsChanged();
            }
        }
    }

    public bool IsGlutenFree
    {
        get => _isGlutenFree;
        set
        {
            if (_isGlutenFree != value)
            {
                _isGlutenFree = value;
                OnPropertyChanged();
                NotifyDerivedMetricsChanged();
            }
        }
    }

    public bool IsDairyFree
    {
        get => _isDairyFree;
        set
        {
            if (_isDairyFree != value)
            {
                _isDairyFree = value;
                OnPropertyChanged();
                NotifyDerivedMetricsChanged();
            }
        }
    }

    public bool NotificationsEnabled
    {
        get => _notificationsEnabled;
        set
        {
            if (_notificationsEnabled != value)
            {
                _notificationsEnabled = value;
                OnPropertyChanged();
                NotifyDerivedMetricsChanged();
            }
        }
    }

    public double TodayStepProgress => Math.Clamp((double)CurrentSteps / StepGoal, 0.0, 1.0);

    public string StepSummaryText => $"{CurrentSteps:N0} / {StepGoal:N0}";

    public int HealthScore => CalculateHealthScore();

    public string DietarySummary => BuildDietarySummary();

    public string NotificationStatus => NotificationsEnabled ? "Reminders on" : "Reminders off";

    public string HealthMessage => HealthScore switch
    {
        >= 85 => $"Excellent momentum. {DietarySummary}. {NotificationStatus}.",
        >= 70 => $"Great progress! {DietarySummary}. {NotificationStatus}.",
        >= 55 => $"Good start. {DietarySummary}. {NotificationStatus}.",
        _ => $"Let's build consistency. {DietarySummary}. {NotificationStatus}."
    };

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void NotifyDerivedMetricsChanged()
    {
        OnPropertyChanged(nameof(TodayStepProgress));
        OnPropertyChanged(nameof(StepSummaryText));
        OnPropertyChanged(nameof(HealthScore));
        OnPropertyChanged(nameof(DietarySummary));
        OnPropertyChanged(nameof(NotificationStatus));
        OnPropertyChanged(nameof(HealthMessage));
    }

    int CalculateHealthScore()
    {
        var stepScore = (int)Math.Round(TodayStepProgress * 90);

        var goalBonus = string.IsNullOrEmpty(HealthGoal) ? 0 : 2;
        var dietBonus = (IsVegan ? 1 : 0) + (IsGlutenFree ? 1 : 0) + (IsDairyFree ? 1 : 0);
        var notifBonus = NotificationsEnabled ? 5 : 0;

        var rawBonus = goalBonus + dietBonus + notifBonus;
        var appliedBonus = TodayStepProgress < 0.5
            ? (int)Math.Round(rawBonus * 0.5)
            : rawBonus;

        return Math.Clamp(stepScore + appliedBonus, 0, 100);
    }

    string BuildDietarySummary()
    {
        var selectedCount = (IsVegan ? 1 : 0) + (IsGlutenFree ? 1 : 0) + (IsDairyFree ? 1 : 0);
        return selectedCount switch
        {
            0 => "No diet filter selected",
            1 => "1 diet preference selected",
            _ => $"{selectedCount} diet preferences selected"
        };
    }
}
