namespace Xaminals.Extensions;

public static class CollectionViewExtenstions
{
    public static bool ClearSelection(this CollectionView cv)
    {
        if (cv.SelectedItem == null)
            return true;

        cv.SelectedItem = null;
        return false;
    }
}

