using System.Windows.Input;
using ControlGallery.Models;

namespace ControlGallery;

public partial class MainPage : ContentPage
{
    readonly ControlCategory allFilter = new("All", Colors.Transparent);
    ControlCategory selectedFilter;
    string searchText = string.Empty;

    public MainPage()
    {
        InitializeComponent();

        FilterOptions = [allFilter, .. ControlCatalog.Categories];
        selectedFilter = allFilter;

        NavigateCommand = new Command<ControlInfo>(async control =>
        {
            var page = (Page)Activator.CreateInstance(control.PageType)!;
            await Navigation.PushAsync(page);
        });

        ApplyFilter();
        BindingContext = this;
    }

    IReadOnlyList<ControlGroup> groups = [];

    public IReadOnlyList<ControlGroup> Groups
    {
        get => groups;
        private set
        {
            groups = value;
            OnPropertyChanged();
        }
    }

    public IReadOnlyList<ControlCategory> FilterOptions { get; }

    public ControlCategory SelectedFilter
    {
        get => selectedFilter;
        set
        {
            selectedFilter = value ?? allFilter;
            OnPropertyChanged();
        }
    }

    public int ControlCount => ControlCatalog.Controls.Count;

    public ICommand NavigateCommand { get; }

    void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        searchText = e.NewTextValue?.Trim() ?? string.Empty;
        ApplyFilter();
    }

    void OnFilterSelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilter();

    void ApplyFilter()
    {
        // Replace the whole list rather than mutating it: simpler and avoids
        // incremental updates of a grouped CollectionView.
        Groups = ControlCatalog.Categories
            .Where(category => selectedFilter == allFilter || selectedFilter == category)
            .Select(category => new ControlGroup(category, ControlCatalog.Controls.Where(c => c.Category == category && MatchesSearch(c))))
            .Where(group => group.Count > 0)
            .ToList();
    }

    bool MatchesSearch(ControlInfo control) =>
        searchText.Length == 0
        || control.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)
        || control.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase);

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        // Give the list more room on short screens, such as a phone in landscape.
        hero.IsVisible = height >= 560;
    }
}
