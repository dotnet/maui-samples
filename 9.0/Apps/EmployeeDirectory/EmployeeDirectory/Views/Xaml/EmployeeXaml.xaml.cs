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
        }
    }

    private void OnFavoriteClicked(object? sender, ToggledEventArgs e)
    {
        if (BindingContext is PersonViewModel personInfo)
        {
            personInfo.ToggleFavorite();
        }
    }

    public void OnItemSelected(object? sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is PersonViewModel.Property property)
        {
            System.Diagnostics.Debug.WriteLine("Property clicked " + property.Type + " " + property.Value);

            switch (property.Type)
            {
                case PersonViewModel.PropertyType.Email:
                    App.PhoneFeatureService?.Email(property.Value);
                    break;
                case PersonViewModel.PropertyType.Twitter:
                    App.PhoneFeatureService?.Tweet(property.Value);
                    break;
                case PersonViewModel.PropertyType.Url:
                    App.PhoneFeatureService?.Browse(property.Value);
                    break;
                case PersonViewModel.PropertyType.Phone:
                    App.PhoneFeatureService?.Call(property.Value);
                    break;
                case PersonViewModel.PropertyType.Address:
                    App.PhoneFeatureService?.Map(property.Value);
                    break;
            }
        }
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}
