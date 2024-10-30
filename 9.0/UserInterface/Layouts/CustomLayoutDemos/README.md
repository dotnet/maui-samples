---
name: .NET MAUI - Custom layouts
description: This sample demonstrates how to create custom layouts in .NET MAUI.
page_type: sample
languages:
- csharp
- xaml
products:
- dotnet-maui
urlFragment: userinterface-customlayouts
---

# Custom layouts

A .NET Multi-platform App UI (.NET MAUI) layout is a list of views with rules and properties that define how to arrange those views within a container. Examples of layouts include `Grid`, `AbsoluteLayout`, and `VerticalStackLayout`.

The process for creating a custom layout in .NET MAUI involves providing an `ILayoutManager` implementation, and overriding the following methods:

- `Size Measure(double widthConstraint, double heightConstraint)`
- `Size ArrangeChildren(Rectangle bounds)`

The `Measure` implementation should call measure on each `IView` in the layout, and should return the total size of the layout given the constraints. The `ArrangeChildren` implementation should determine where each `IView` should be placed within the given bounds, and should call `Arrange` on each `IView` with its appropriate bounds. The return value should be the actual size of the layout.

This sample demonstrates the following custom layouts:

- `CascadeLayout`, which cascades items from top left to bottom right, similar to using the cascade windows arrangement in an MDI application.
- `ColumnLayout`, which is similar to the Xamarin.Forms `*AndExpand` properties. It's a subclass of `VerticalStackLayout`, which adds a `Fill` attached property that can be applied to one or more children of the layout. It uses a custom layout manager that converts the `VerticalStackLayout` into a single-column `Grid` at runtime. Each `VerticalStackLayout` child gets its own row in the `Grid`. The rows are set to a height of `Auto`, but children marked as `Fill` receive a row height of `*` instead.
- `ContentColumnLayout`, which is a custom layout that displays a header, content, and footer. It subclasses `Layout` and implements some extra properties and methods from the `IGridLayout` interface. This enables it to be passed into the `GridLayoutManager` to handle the layout at runtime.
- `HorizontalWrapLayout`, which works like a horizontal stack layout, except that instead of extending out as far as it needs to the right, it will wrap to a new row when it encounters the right edge of its container.
- `ZStackLayout`, which is a variation of a `StackLayout` that arranges its children on top of each other. All its children are laid out at the origin. The arrangement area's width is determined by the widest child and the height is determined by the tallest child.

In some situations you may find that you want to change the behavior of an existing layout type without having to create a custom layout. For those scenarios you can create an `ILayoutManagerFactory` and use it to replace the default layout manager type with your own. The `CustomizedGridLayoutManager` type in the sample demonstrates doing this.

> [!NOTE]
> These layouts are not intended as production-ready layouts.

For more information about custom layouts, see [Custom layouts](https://learn.microsoft.com/dotnet/maui/user-interface/layouts/custom).
