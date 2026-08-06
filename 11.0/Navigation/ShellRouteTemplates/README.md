---
name: .NET MAUI - Shell route templates
description: Demonstrates absolute Shell navigation with required, optional, defaulted, constrained, catch-all, and mixed route-template parameters.
page_type: sample
languages:
- csharp
- xaml
products:
- dotnet-maui
urlFragment: navigation-shell-route-templates
---

# Shell route templates

This .NET MAUI 11 sample is a small trip-planning route lab. Its route matrix runs every route-template form introduced by [dotnet/maui#35110](https://github.com/dotnet/maui/pull/35110), navigates to a result page, and compares the delivered parameter with the expected value.

> [!IMPORTANT]
> Route templates support **absolute navigation only** in this release. Every example uses a URI beginning with `//routes`. Do not use these templates with relative navigation.

## What you'll learn

- How to register required, optional, defaulted, constrained, catch-all, and mixed route templates.
- Which constraints are implemented by the shipping parser.
- How path parameters flow through both `[QueryProperty]` and `IQueryAttributable`.
- How to verify the resolved value on the destination page.

## Requirements

- .NET SDK `11.0.100-preview.7.26381.103`
- .NET MAUI workload
- .NET MAUI `11.0.0-preview.7.26404.4`, supplied by `11.0/Directory.Build.props`
- Android, iOS, or Mac Catalyst tooling for the target you run

## Route matrix

Optional and default parameters must be the final segment. Catch-all parameters must also be last. The shipping implementation supports one parameter per mixed segment and one constraint per parameter.

| Form | Registered template | Absolute URI used by the sample | Delivered value |
|---|---|---|---|
| Required | `trip/{tripId}` | `//routes/trip/SEA-204` | `tripId = SEA-204` |
| Optional, present | `traveler/{name?}` | `//routes/traveler/Ada` | `name = Ada` |
| Optional, absent | `traveler/{name?}` | `//routes/traveler` | `name` is not supplied |
| Default | `rating/{stars=5}` | `//routes/rating` | `stars = 5` |
| `int` constraint | `reservation/{reservationId:int}` | `//routes/reservation/42` | `reservationId = 42` |
| `long` constraint | `loyalty/{points:long}` | `//routes/loyalty/9000000000` | `points = 9000000000` |
| `double` constraint | `budget/{amount:double}` | `//routes/budget/1299.50` | `amount = 1299.50` |
| `bool` constraint | `toggle/{enabled:bool}` | `//routes/toggle/true` | `enabled = true` |
| `guid` constraint | `booking/{reference:guid}` | `//routes/booking/550e8400-e29b-41d4-a716-446655440000` | `reference` is the GUID |
| `alpha` constraint | `region/{name:alpha}` | `//routes/region/Pacific` | `name = Pacific` |
| Catch-all | `files/{*path}` | `//routes/files/trips/SEA-204/receipt.pdf` | `path = trips/SEA-204/receipt.pdf` |
| Mixed segment | `trip-{tripId}-summary` | `//routes/trip-SEA-204-summary` | `tripId = SEA-204` |

The app appends a `caseId` query string solely to select the expected matrix row. The values shown above come from the path template.

## Key files

| File | Purpose |
|---|---|
| `src/AppShell.xaml.cs` | Registers each route template. |
| `src/Models/RouteCatalog.cs` | Defines the testable route matrix and expected values. |
| `src/ViewModels/MainPageViewModel.cs` | Executes each absolute navigation URI. |
| `src/QueryPropertyResultPage.xaml.cs` | Receives required and mixed parameters through `[QueryProperty]`. |
| `src/AttributableResultPage.xaml.cs` | Receives the other parameters through `IQueryAttributable`. |

## Run the sample

From this directory:

```bash
dotnet build src/ShellRouteTemplates.sln
dotnet build -t:Run -f net11.0-maccatalyst src/ShellRouteTemplates.csproj
```

You can also select the `net11.0-ios` target and an iOS simulator in Visual Studio Code or Visual Studio. On the route matrix, choose **Run** for each row. The destination page displays `PASS` when the actual path parameter matches the expected value.

## Resources

- [Feature PR: Shell route templates with path parameters](https://github.com/dotnet/maui/pull/35110)
- [Shipping route-template parser](https://github.com/dotnet/maui/blob/e45600b065c6636c73fefdc8406bf8881f65e9d4/src/Controls/src/Core/Shell/RouteTemplate.cs)
- [Shipping route-template tests](https://github.com/dotnet/maui/blob/e45600b065c6636c73fefdc8406bf8881f65e9d4/src/Controls/tests/Core.UnitTests/ShellRouteTemplatesTests.cs)
- [.NET MAUI for .NET 11 release notes](https://learn.microsoft.com/dotnet/maui/whats-new/dotnet-11?view=net-maui-11.0)
