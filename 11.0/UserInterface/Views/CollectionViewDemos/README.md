---
name: .NET MAUI - CollectionView
description: This sample demonstrates how to use the .NET MAUI CollectionView.
page_type: sample
languages:
- csharp
- xaml
products:
- dotnet-maui
urlFragment: userinterface-collectionview
---

# CollectionView

The .NET Multi-platform App UI (.NET MAUI) `CollectionView` is a view for presenting lists of data using different layout specifications. It aims to provide a more flexible, and performant alternative to `ListView`.

This sample demonstrates how to use the .NET MAUI `CollectionView`.

For more information about this sample, see [.NET MAUI CollectionView](https://docs.microsoft.com/dotnet/maui/user-interface/controls/collectionview/).

## Coming from ListViewDemos

`ListView` is deprecated in .NET MAUI, so the .NET 11 samples no longer include ListViewDemos. This sample covers the same scenarios with `CollectionView`. The original sample remains in [10.0/UserInterface/Views/ListViewDemos](../../../../10.0/UserInterface/Views/ListViewDemos) for reference.

| ListViewDemos page | CollectionViewDemos equivalent |
|---|---|
| TextListPage | Layout/VerticalListTextPage |
| VerticalListPage | Layout/VerticalListPage |
| DataTemplateSelectorPage | Layout/VerticalListDataTemplateSelectorPage |
| SelectionPage | Selection/VerticalListSingleSelectionPage and the other Selection pages |
| RuntimeItemSizingPage (`ViewCell.ForceUpdateSize`) | Sizing/VerticalListDynamicSizeItemsPage |
| GroupingPage | Grouping/VerticalListGroupingPage |
| ContextMenuItemsPage (`ViewCell.ContextActions`) | Swipe/VerticalListSwipeContextItemsPage: `SwipeView` on touch platforms, `FlyoutBase.ContextFlyout` for right-click on Mac Catalyst and Windows |
| PullToRefreshPage | PullToRefresh/VerticalListPullToRefreshPage (`RefreshView`) |
| RightToLeftListPage | Layout/VerticalListRTLPage |
| ScrollByItemPage | Scrolling/ScrollToByObjectPage |
| ScrollByItemWithGroupingPage | Scrolling/ScrollToByObjectWithGroupingPage |
| TextHeaderFooterPage | HeadersAndFooters/VerticalListHeaderFooterStringPage |
| ViewHeaderFooterPage | HeadersAndFooters/VerticalListHeaderFooterViewPage |
| TemplatedHeaderFooterPage | HeadersAndFooters/VerticalListHeaderFooterDataTemplatePage |

Pages are under [CollectionViewDemos/Views](CollectionViewDemos/Views).
