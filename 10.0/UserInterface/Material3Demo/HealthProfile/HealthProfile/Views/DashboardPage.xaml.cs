using HealthProfile.ViewModels;

namespace HealthProfile.Views;

public partial class DashboardPage : ContentPage
{
    AppViewModel? _viewModel;

    public DashboardPage()
    {
        InitializeComponent();
        _viewModel = IPlatformApplication.Current?.Services.GetService<AppViewModel>();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel is not null)
        {
            _viewModel.PropertyChanged += OnViewModelChanged;
            RefreshDisplay();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (_viewModel is not null)
        {
            _viewModel.PropertyChanged -= OnViewModelChanged;
        }
    }

    void OnViewModelChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        RefreshDisplay();
    }

    void RefreshDisplay()
    {
        if (_viewModel is not null)
        {
            StepProgressBar.Progress = _viewModel.TodayStepProgress;
            StepsLabel.Text = _viewModel.StepSummaryText;

            HealthScoreLabel.Text = $"{_viewModel.HealthScore} / 100";
            HealthMessageLabel.Text = _viewModel.HealthMessage;
            HealthScoreProgressBar.Progress = _viewModel.HealthScore / 100.0;
        }
    }
}
