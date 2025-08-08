## Context

A flatter visual tree is better since it reduces the number of measure and arrange passes needed to render the tree. For this reason, prefer a complex Grid layout over nesting multiple other layouts.

## Guidelines

Do not nest scrollable controls (ScrollView, CollectionView, ListView) within each other unless they scroll in different directions. It is OK to nest within a RefreshView or similar pull-to-refresh control.

### Handling Layout Changes
- **Good Practice**: Override `OnSizeAllocated` to handle layout changes and adjust child elements accordingly.
- **Bad Practice**: Not handling layout changes or using event handlers for layout changes.

```csharp
// Good Practice
public class CustomLayout : StackLayout
{
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        // Adjust child elements based on new size
    }
}

// Bad Practice
public class CustomLayout : StackLayout
{
    public CustomLayout()
    {
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        // Adjust child elements based on new size
    }
}
```