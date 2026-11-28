using System.Windows.Input;
using DataBindingDemos.Models;

namespace DataBindingDemos;

public partial class MainPage : ContentPage
{
    readonly DemoCategory allFilter = new("All", Colors.Transparent);
    DemoCategory selectedFilter;
    string searchText = string.Empty;

    public MainPage()
    {
        InitializeComponent();

        FilterOptions = [allFilter, .. DemoCatalog.Categories];
        selectedFilter = allFilter;

        NavigateCommand = new Command<DemoInfo>(async demo =>
        {
            var page = (Page)Activator.CreateInstance(demo.PageType)!;
            await Navigation.PushAsync(page);
        });

        ApplyFilter();
        BindingContext = this;
    }

    IReadOnlyList<DemoGroup> groups = [];

    public IReadOnlyList<DemoGroup> Groups
    {
        get => groups;
        private set
        {
            groups = value;
            OnPropertyChanged();
        }
    }

    public IReadOnlyList<DemoCategory> FilterOptions { get; }

    public DemoCategory SelectedFilter
    {
        get => selectedFilter;
        set
        {
            selectedFilter = value ?? allFilter;
            OnPropertyChanged();
        }
    }

    public int DemoCount => DemoCatalog.Demos.Count;

    public ICommand NavigateCommand { get; }

    void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        searchText = e.NewTextValue?.Trim() ?? string.Empty;
        ApplyFilter();
    }

    void OnFilterSelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilter();

    void ApplyFilter()
    {
        // Replace the whole list rather than mutating it: simpler, and avoids
        // incremental updates of a grouped CollectionView.
        Groups = DemoCatalog.Categories
            .Where(category => selectedFilter == allFilter || selectedFilter == category)
            .Select(category => new DemoGroup(category, DemoCatalog.Demos.Where(d => d.Category == category && MatchesSearch(d))))
            .Where(group => group.Count > 0)
            .ToList();
    }

    bool MatchesSearch(DemoInfo demo) =>
        searchText.Length == 0
        || demo.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase)
        || demo.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase)
        || demo.PageType.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        // Give the list more room on short screens, such as a phone in landscape.
        hero.IsVisible = height >= 560;
    }
}
