# InvalidateStyle Demo

Demonstrates two new .NET MAUI 11 APIs for dynamically reapplying styles after mutating setter values at runtime:

- **`StyleableElement.InvalidateStyle()`** — mutate a `Style` object's `Setter.Value`, then call `InvalidateStyle()` on the element to reapply the updated style.
- **`VisualStateManager.InvalidateVisualStates(VisualElement)`** — mutate setter values inside a `VisualState`, then call `InvalidateVisualStates()` to reapply the current visual state with the new values.

## What it shows

1. A **Button** with an explicit `Style` (blue background, font size 18). Tapping "Change Style" toggles the background to orange-red and increases font size, then calls `InvalidateStyle()`.
2. A **Label** with `VisualStateManager` states (Normal / PointerOver). Tapping "Change Visual State Colors" mutates the state setters and calls `InvalidateVisualStates()`.

## APIs

| API | Namespace | Since |
|-----|-----------|-------|
| `StyleableElement.InvalidateStyle()` | `Microsoft.Maui.Controls` | .NET 11 Preview 3 |
| `VisualStateManager.InvalidateVisualStates(VisualElement)` | `Microsoft.Maui.Controls` | .NET 11 Preview 3 |

## Reference

- [dotnet/maui#34723](https://github.com/dotnet/maui/pull/34723)
