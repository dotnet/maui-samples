using DeveloperBalance.Models;

namespace DeveloperBalance.Pages;

public partial class ProjectDetailPage : ContentPage
{
    private readonly ProjectDetailPageModel _model;

    public ProjectDetailPage(ProjectDetailPageModel model)
    {
        InitializeComponent();
        _model = model;
        BindingContext = model;
    }

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView)
        {
            var cur = e.CurrentSelection;
            var prev = e.PreviousSelection;

            // TODO - Does the Current and Previous not work as expected on iOS?

            var changedTag = cur.Except(prev).ToList();
            if (changedTag.Count == 0)
            {
                changedTag = prev.Except(cur).ToList();
            }

            if (changedTag.Count == 1 && changedTag[0] is Tag tag)
            {
                await _model.ToggleTag(tag);
            }
        }
    }
}