
# TableView

TableView is deprecated and should not be used in new apps. It is going
to be removed in a future release of .NET MAUI.

TableView is not virtualized and it isn't designed to support binding to
a collection of items. Replace with a `VerticalStackLayout`.

When replacing the items in the TableView, the section headings and
groupings should be preserved.

Cells should not be replaced with `Button`, use a tap gesture recognizer
on the view or layout that represents the cell.
