using EmployeeDirectory.Core.ViewModels;

namespace EmployeeDirectory.Views.Xaml;

public partial class EmployeeXaml : ContentPage
{
    public EmployeeXaml()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        favoriteSwitch.Toggled += OnFavoriteClicked;
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        if (BindingContext is PersonViewModel personInfo)
        {
            Title = personInfo.Person.Name;
            favoriteLabel.Text = personInfo.FavoriteStatusText;
        }
    }

    private void OnFavoriteClicked(object? sender, ToggledEventArgs e)
    {
        if (BindingContext is PersonViewModel personInfo)
        {
            personInfo.ToggleFavorite();
            favoriteLabel.Text = personInfo.FavoriteStatusText;
        }
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}
