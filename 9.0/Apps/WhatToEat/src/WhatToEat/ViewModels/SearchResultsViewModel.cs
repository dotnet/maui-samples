using Recipes.Views;

namespace Recipes.ViewModels
{
    [QueryProperty(nameof(SearchQuery), nameof(SearchQuery))]
    [QueryProperty(nameof(SearchFilter), nameof(SearchFilter))]
    public class SearchResultsViewModel : BaseViewModel
    {
        MockRestService _restService;

        RecipeData _recipeData;
        string _searchQuery;
        string _noResultsLabel;
        bool _noResultsLabelVisible;
        bool _searchResultsVisible;
        Hit _selectedHit;

        public Command<Hit> ItemTapped { get; }
        public Command SearchCommand { get; }

        public SearchResultsViewModel()
        {
            Title = "Search all recipes";
            _restService = new MockRestService();
            NoResultsLabelVisible = false;
            SearchResultsVisible = true;

            ItemTapped = new Command<Hit>(OnItemSelected);
            SearchCommand = new Command(async () => await OnSearch());

		}

        public RecipeData RecipeData
        {
            get => _recipeData;
            set => SetProperty(ref _recipeData, value);
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set => SetProperty(ref _searchQuery, value);
        }

        public string SearchFilter { get; set; }

        public string NoResultsLabel
        {
            get => _noResultsLabel;
            set => SetProperty(ref _noResultsLabel, value);
        }

        public bool NoResultsLabelVisible
        {
            get => _noResultsLabelVisible;
            set => SetProperty(ref _noResultsLabelVisible, value);
        }

        public bool SearchResultsVisible
        {
            get => _searchResultsVisible;
            set => SetProperty(ref _searchResultsVisible, value);
        }

        public Hit SelectedHit
        {
            get
            {
                return _selectedHit;
            }
            set
            {
                if (_selectedHit != value)
                {
                    _selectedHit = value;
                    OnItemSelected(_selectedHit);
                }
            }
        }

        async Task OnSearch()
        {
            NoResultsLabelVisible = false;

            if (!string.IsNullOrWhiteSpace(SearchQuery) || !string.IsNullOrWhiteSpace(SearchFilter))
            {
                RecipeData recipeData = await _restService.GetRecipeDataAsync(SearchFilter);

                if (recipeData == null || recipeData.Hits.Length == 0)
                {
                    NoResultsLabel = $"Sorry! We couldn't find any recipes for {SearchQuery}. Try searching for a different recipe!";
                    NoResultsLabelVisible = true;
                    SearchResultsVisible = false;
                }
                else
                {
                    NoResultsLabelVisible = false;
                    SearchResultsVisible = true;

                    for (int i = 0; i < recipeData.Hits.Length; i++)
                    {
                        recipeData.Hits[i].Id = i;
                    }

                    RecipeData = recipeData;
                    AppShell.Data = RecipeData;
                }
            }
        }

        async void OnItemSelected(Hit hit)
        {
            if (hit == null)
                return;

            // This will push the SearchResultDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(SearchResultDetailPage)}?HitId={hit.Id}");
        }
    }
}
