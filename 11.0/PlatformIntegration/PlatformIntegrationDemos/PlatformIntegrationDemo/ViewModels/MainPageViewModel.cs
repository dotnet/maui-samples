using PlatformIntegrationDemo.Models;

namespace PlatformIntegrationDemo.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        readonly SampleCategory allFilter = new("All", Colors.Transparent);
        bool alreadyAppeared;
        IReadOnlyList<SampleGroup> groups = [];
        SampleCategory selectedFilter;
        string filterText = string.Empty;

        public MainPageViewModel()
        {
            FilterOptions = [allFilter, .. SampleCatalog.Categories];
            selectedFilter = allFilter;
            ApplyFilter();
        }

        public IReadOnlyList<SampleCategory> FilterOptions { get; }

        public int SampleCount => SampleCatalog.Samples.Count;

        public IReadOnlyList<SampleGroup> Groups
        {
            get => groups;
            private set => SetProperty(ref groups, value);
        }

        public string FilterText
        {
            get => filterText;
            set => SetProperty(ref filterText, value, onChanged: ApplyFilter);
        }

        public SampleCategory SelectedFilter
        {
            get => selectedFilter;
            set => SetProperty(ref selectedFilter, value ?? allFilter, onChanged: ApplyFilter);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            if (!alreadyAppeared)
            {
                alreadyAppeared = true;

                if (VersionTracking.IsFirstLaunchEver)
                {
                    DisplayAlertAsync("Welcome to the sample! This is the first time you are launching the app!");
                }
                else if (VersionTracking.IsFirstLaunchForCurrentVersion)
                {
                    var count = VersionTracking.VersionHistory.Count();
                    DisplayAlertAsync($"Welcome to the NEW sample! You have tried {count} versions.");
                }
            }
        }

        void ApplyFilter()
        {
            var search = filterText?.Trim() ?? string.Empty;

            // Replace the whole list rather than mutating it: simpler, and it avoids
            // incremental updates of a grouped CollectionView.
            Groups = SampleCatalog.Categories
                .Where(category => selectedFilter == allFilter || selectedFilter == category)
                .Select(category => new SampleGroup(category, SampleCatalog.Samples
                    .Where(s => s.Category == category && Matches(s, search))
                    .OrderBy(s => s.Name)))
                .Where(group => group.Count > 0)
                .ToList();
        }

        static bool Matches(SampleItem sample, string search) =>
            search.Length == 0
            || sample.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
            || sample.Description.Contains(search, StringComparison.OrdinalIgnoreCase)
            || sample.Tags.Any(t => t.Contains(search, StringComparison.OrdinalIgnoreCase));
    }
}
