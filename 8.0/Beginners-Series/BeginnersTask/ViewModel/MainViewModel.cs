using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BeginnersTasks.ViewModel;

public partial class MainViewModel : ObservableObject
{
    readonly IConnectivity connectivity;
    public MainViewModel(IConnectivity connectivity)
    {
        Items = [];
        this.connectivity = connectivity;
    }

    [ObservableProperty]
    ObservableCollection<string> items;

    [ObservableProperty]
    string text;

    [RelayCommand]
    async Task Add()
    {
        // Assure text is not empty
        if (string.IsNullOrWhiteSpace(Text))
            return;

        // Assure there's an internet connection
        // else show an alert
        if(connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            await Shell.Current.DisplayAlert("Uh Oh!", "No Internet", "OK");
            return;
        }

        // Add text to list of todos
        Items.Add(Text);
        
        // Reset Text
        Text = string.Empty;
    }

    [RelayCommand]
    void Delete(string s)
    {
        // If the list of todos contains
        // given string, remove it from list
        if(Items.Contains(s))
        {
            Items.Remove(s);
        }
    }

    [RelayCommand]
    async Task Tap(string s)
    {
        // Trigger a navigation to the detail page
        //  - See [AppShell] for how to add a routing to the app's navigation
        //  - See [DetailViewModel] for how to resolve the 'Text' query parameter
        await Shell.Current.GoToAsync($"{nameof(DetailPage)}?Text={s}");
    }
}
