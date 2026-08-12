---
name: .NET MAUI - Back button accessibility
description: "Demonstrates the new back button accessibility label APIs on NavigationPage and Shell's BackButtonBehavior in .NET MAUI 11."
page_type: sample
languages:
- csharp
- xaml
products:
- dotnet-maui
urlFragment: fundamentals-backbutton-accessibility
---
# Back button accessibility

This sample demonstrates the new accessibility-label APIs for the navigation back button introduced in .NET MAUI 11 ([dotnet/maui#35011](https://github.com/dotnet/maui/pull/35011)).

In prior versions, the back button on `NavigationPage` and Shell announced a generic platform string ("Back") to screen readers. .NET MAUI 11 adds two new properties so an app can set a custom announcement:

- `NavigationPage.BackButtonAccessibilityLabel` — attached property used on any page hosted in a `NavigationPage`.
- `Shell.BackButtonBehavior.AccessibilityLabel` — `BindableProperty` on `BackButtonBehavior` for Shell pages.

Both are surfaced to **VoiceOver** (iOS / Mac Catalyst), **TalkBack** (Android), and **Narrator** (Windows).

## Features demonstrated

- A `NavigationPage`-hosted detail page with a custom `NavigationPage.BackButtonAccessibilityLabel` so screen readers announce "Back to order list" instead of "Back".

### Shell variant (XAML reference)

For Shell, set the label on a `BackButtonBehavior`:

```xml
<Shell ...>
    <Shell.BackButtonBehavior>
        <BackButtonBehavior AccessibilityLabel="Back to order list" />
    </Shell.BackButtonBehavior>
    ...
</Shell>
```

## Project structure

| File | Purpose |
|------|---------|
| `App.xaml.cs` | Wraps `MainPage` in a `NavigationPage` |
| `MainPage.xaml` | Root order list page |
| `OrderDetailPage.xaml` | Detail page that sets `NavigationPage.BackButtonAccessibilityLabel` |

## Prerequisites

- .NET 11 Preview 5 or later
- .NET MAUI workload (`dotnet workload install maui`)

## Running the sample

```bash
dotnet build -t:Run -f net11.0-maccatalyst
```

## Try this

1. Tap **View order #1138 details** to push the detail page.
2. Enable a screen reader (VoiceOver, TalkBack, or Narrator).
3. Focus the back button at the top of the detail page — you should hear "Back to order list" instead of the platform default.
