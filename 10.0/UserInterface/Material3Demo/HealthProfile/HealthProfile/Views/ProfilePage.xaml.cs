using System.ComponentModel;
using HealthProfile.ViewModels;
namespace HealthProfile.Views;

public partial class ProfilePage : ContentPage
{
    const string DefaultProfileImage = "dotnet_bot.svg";

    AppViewModel? _viewModel;

    public ProfilePage()
    {
        InitializeComponent();
        _viewModel = IPlatformApplication.Current?.Services.GetService<AppViewModel>();

        BloodTypePicker.SelectedIndex = 4;
        DobPicker.Date = new DateTime(2001, 6, 18);
        MorningReminderPicker.Time = new TimeSpan(7, 30, 0);
        ReminderTimeLabel.Text = "07:30 AM";
        ProfileImage.Source = DefaultProfileImage;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SyncIndicator.IsRunning = false;
        SyncIndicator.IsVisible = false;
        SyncStatusLabel.Text = "Health data is up to date";

        if (_viewModel is not null)
        {
            var goalInThousands = Math.Clamp((int)Math.Round(_viewModel.StepGoal / 1000.0), 1, 10);
            var goalInSteps = goalInThousands * 1000;

            StepGoalSlider.Value = goalInThousands;
            _viewModel.StepGoal = goalInSteps;
            UpdateStepGoalDisplay(goalInSteps, _viewModel.CurrentSteps);

            VeganCheck.IsChecked = _viewModel.IsVegan;
            GlutenFreeCheck.IsChecked = _viewModel.IsGlutenFree;
            DairyFreeCheck.IsChecked = _viewModel.IsDairyFree;
            NotificationsSwitch.IsToggled = _viewModel.NotificationsEnabled;

            RadioWeightLoss.IsChecked = _viewModel.HealthGoal == "Weight Loss";
            RadioMuscleGain.IsChecked = _viewModel.HealthGoal == "Muscle Gain";
            RadioMaintenance.IsChecked = _viewModel.HealthGoal == "Maintenance";
        }

        ApplyProfileSearch(ProfileSearchBar.Text);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        SyncIndicator.IsRunning = false;
    }

    void OnStepGoalChanged(object sender, ValueChangedEventArgs e)
    {
        var goalInThousands = Math.Clamp((int)Math.Round(e.NewValue), 1, 10);
        var goalInSteps = goalInThousands * 1000;
        var currentSteps = _viewModel?.CurrentSteps ?? 0;
        UpdateStepGoalDisplay(goalInSteps, currentSteps);
    }

    void OnStepGoalDragCompleted(object sender, EventArgs e)
    {
        var goalInThousands = Math.Clamp((int)Math.Round(StepGoalSlider.Value), 1, 10);
        var goalInSteps = goalInThousands * 1000;

        StepGoalSlider.Value = goalInThousands;

        if (_viewModel is not null)
        {
            _viewModel.StepGoal = goalInSteps;
        }

        var currentSteps = _viewModel?.CurrentSteps ?? 0;
        UpdateStepGoalDisplay(goalInSteps, currentSteps);
    }

    void OnReminderTimeChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TimePicker.Time))
        {
            var dt = DateTime.Today.Add(MorningReminderPicker.Time ?? TimeSpan.Zero);
            ReminderTimeLabel.Text = dt.ToString("hh:mm tt").ToUpperInvariant();
        }
    }

    void OnProfileSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        SearchSuggestionsCard.IsVisible = string.IsNullOrWhiteSpace(e.NewTextValue) && ProfileSearchBar.IsFocused;
        ApplyProfileSearch(e.NewTextValue);
    }

    void OnProfileSearchFocused(object sender, FocusEventArgs e)
    {
        SearchSuggestionsCard.IsVisible = string.IsNullOrWhiteSpace(ProfileSearchBar.Text);
    }

    void OnProfileSearchUnfocused(object sender, FocusEventArgs e)
    {
        SearchSuggestionsCard.IsVisible = false;
    }

    void OnProfileSearchButtonPressed(object sender, EventArgs e)
    {
        ApplyProfileSearch(ProfileSearchBar.Text);
        SearchSuggestionsCard.IsVisible = false;
        ProfileSearchBar.Unfocus();
    }

    void OnSearchSuggestionClicked(object sender, EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        ProfileSearchBar.Text = button.Text;
        ApplyProfileSearch(button.Text);
        SearchSuggestionsCard.IsVisible = false;
        ProfileSearchBar.Unfocus();
    }

    void ApplyProfileSearch(string? query)
    {
        var queryText = query?.Trim();
        if (string.IsNullOrEmpty(queryText))
        {
            ProfilePhotoSection.IsVisible = true;
            PersonalInfoCard.IsVisible = true;
            PersonalInfoHeader.IsVisible = true;
            StepGoalCard.IsVisible = true;
            ActivityGoalsHeader.IsVisible = true;
            ReminderCard.IsVisible = true;
            RemindersHeader.IsVisible = true;
            SyncCard.IsVisible = true;
            DataSyncHeader.IsVisible = true;
            HealthGoalCard.IsVisible = true;
            HealthTargetsHeader.IsVisible = true;
            DietaryCard.IsVisible = true;
            DietPreferencesHeader.IsVisible = true;
            NotificationsCard.IsVisible = true;
            AlertsHeader.IsVisible = true;
            NotesCard.IsVisible = true;
            MedicalNotesHeader.IsVisible = true;
            return;
        }

        bool MatchAny(params string[] keywords)
        {
            return keywords.Any(k =>
                k.Contains(queryText, StringComparison.OrdinalIgnoreCase) ||
                queryText.Contains(k, StringComparison.OrdinalIgnoreCase));
        }

        bool personalInfoMatch = MatchAny("personal info", "personal information", "name", "full name", "dob", "date of birth", "birth", "blood", "blood type");
        PersonalInfoCard.IsVisible = personalInfoMatch;
        PersonalInfoHeader.IsVisible = personalInfoMatch;

        bool activityGoalsMatch = MatchAny("activity goals", "activity goal", "step goal", "step", "goal", "progress", "activity");
        StepGoalCard.IsVisible = activityGoalsMatch;
        ActivityGoalsHeader.IsVisible = activityGoalsMatch;

        bool remindersMatch = MatchAny("reminders", "reminder", "time", "alarm", "morning");
        ReminderCard.IsVisible = remindersMatch;
        RemindersHeader.IsVisible = remindersMatch;

        bool syncMatch = MatchAny("data sync", "sync", "refresh", "data");
        SyncCard.IsVisible = syncMatch;
        DataSyncHeader.IsVisible = syncMatch;

        bool healthGoalMatch = MatchAny("health targets", "health goal", "goal", "weight", "weight loss", "muscle", "muscle gain", "maintenance");
        HealthGoalCard.IsVisible = healthGoalMatch;
        HealthTargetsHeader.IsVisible = healthGoalMatch;

        bool dietMatch = MatchAny("diet preferences", "diet", "dietary", "vegan", "gluten", "gluten-free", "dairy", "dairy-free");
        DietaryCard.IsVisible = dietMatch;
        DietPreferencesHeader.IsVisible = dietMatch;

        bool notificationsMatch = MatchAny("alerts", "notifications", "notification", "alert", "reminder");
        NotificationsCard.IsVisible = notificationsMatch;
        AlertsHeader.IsVisible = notificationsMatch;

        bool notesMatch = MatchAny("medical notes", "notes", "history", "medical");
        NotesCard.IsVisible = notesMatch;
        MedicalNotesHeader.IsVisible = notesMatch;

        ProfilePhotoSection.IsVisible = MatchAny("profile picture", "photo", "avatar", "image", "picture");
    }

    void OnHealthGoalChanged(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value)
        {
            return;
        }

        var radio = sender as RadioButton;
        if (radio is not null && _viewModel is not null)
        {
            _viewModel.HealthGoal = radio.Content?.ToString() ?? "Weight Loss";
        }
    }

    void OnDietaryChanged(object sender, CheckedChangedEventArgs e)
    {
        if (_viewModel is null)
        {
            return;
        }

        _viewModel.IsVegan = VeganCheck.IsChecked;
        _viewModel.IsGlutenFree = GlutenFreeCheck.IsChecked;
        _viewModel.IsDairyFree = DairyFreeCheck.IsChecked;
    }

    void OnNotificationsToggled(object sender, ToggledEventArgs e)
    {
        if (_viewModel is null)
        {
            return;
        }

        _viewModel.NotificationsEnabled = e.Value;
    }

    async void OnSaveProfileClicked(object sender, EventArgs e)
    {
        SaveProfileButton.IsEnabled = false;
        SaveProfileButton.Text = "Saving...";

        await Task.Delay(500);

        SaveProfileButton.Text = "SAVE PROFILE";
        SaveProfileButton.IsEnabled = true;

        await DisplayAlertAsync("Saved", "Profile preview updated.", "OK");
    }

    void UpdateStepGoalDisplay(int goalInSteps, int currentSteps)
    {
        var goalInThousands = goalInSteps / 1000;
        StepGoalValueLabel.Text = $"{goalInThousands}k";
        StepProgressBar.Progress = Math.Min(1.0, (double)currentSteps / goalInSteps);
        StepProgressLabel.Text = $"{currentSteps:N0} STEPS";
    }

    async void OnEditPhotoBtnClicked(object sender, EventArgs e)
    {
        try
        {
#pragma warning disable CS0618
            var photos = await MediaPicker.Default.PickPhotosAsync();
#pragma warning restore CS0618
            if (photos is null)
            {
                return;
            }

            FileResult? photo = null;
            foreach (var item in photos)
            {
                photo = item;
                break;
            }

            if (photo is null)
            {
                return;
            }

            var extension = Path.GetExtension(photo.FileName);
            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".jpg";
            }

            var tempPath = Path.Combine(FileSystem.Current.CacheDirectory, $"profile-preview{extension}");
            await using var sourceStream = await photo.OpenReadAsync();
            await using var targetStream = File.Create(tempPath);
            await sourceStream.CopyToAsync(targetStream);

            ProfileImage.Source = ImageSource.FromFile(tempPath);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Photo Picker", $"Unable to pick photo: {ex.Message}", "OK");
        }
    }

    async void OnRefreshSyncClicked(object sender, EventArgs e)
    {
        SyncIndicator.IsVisible = true;
        SyncIndicator.IsRunning = true;
        SyncStatusLabel.Text = "Syncing health data...";
        RefreshSyncBtn.IsEnabled = false;

        try
        {
            await Task.Delay(1200);
            LastSyncLabel.Text = $"Last sync: {DateTime.Now:hh:mm tt}";
            SyncStatusLabel.Text = "Health data is up to date";
        }
        finally
        {
            SyncIndicator.IsRunning = false;
            SyncIndicator.IsVisible = false;
            RefreshSyncBtn.IsEnabled = true;
        }
    }
}
