You are an agent - please keep going until the user's query is completely resolved, before ending your turn and yielding back to the user. Only terminate your turn when you are sure that the problem is solved.

If you are not sure about file content or codebase structure pertaining to the user's request, use your tools to read files and gather the relevant information: do NOT guess or make up an answer.

You MUST plan extensively before each function call, and reflect extensively on the outcomes of the previous function calls. DO NOT do this entire process by making function calls only, as this can impair your ability to solve the problem and think insightfully.

When in Agent mode, work directly in the code files.

## NuGet Dependencies

- Prefer the latest stable release versions of NuGet dependencies when adding or updating packages.
- If choosing the latest stable diverges from versions used elsewhere in this repository, call it out to the user with a brief note summarizing the differences before proceeding (or in the same turn when making the change).
- Common .NET MAUI package recommendations:
  - `Microsoft.Maui.Controls` - Core MAUI controls
  - `CommunityToolkit.Maui` - Essential community extensions
  - `CommunityToolkit.Mvvm` - MVVM helpers and source generators

## About the Project

This application is a .NET MAUI mobile and desktop application that helps users organize their "to do" lists into projects.

The solution file is in the `/src` folder, and the project file is in the `/src/Telepathic` folder. When issuing a `dotnet build` command you must include a Target Framework Moniker like `dotnet build -f net9.0-maccatalyst`. Use the TFM that VS is currently targeting, or if you cannot read that use the version in the csproj file.

## .NET MAUI Best Practices

### Layout Controls

- Use `Border` instead of `Frame` (Frame is obsolete in .NET 9)
- Use `Grid` instead of `StackLayout` for complex layouts requiring space subdivision
- Use `HorizontalStackLayout` or `VerticalStackLayout` for simple linear layouts
- Use `CollectionView` instead of `ListView` for lists of greater than 20 items that should be virtualized
- Use `BindableLayout` with an appropriate layout inside a `ScrollView` for items of 20 or less that don't need to be virtualized

### Visual Properties

- Use `Background` (Brush) instead of `BackgroundColor` (Color)
- Use `Stroke` instead of `BorderColor` for Border controls
- Use explicit numeric font sizes instead of `Device.GetNamedSize()` (deprecated)

### Layout Options

- **CRITICAL**: Do NOT use `AndExpand` layout options (e.g., `FillAndExpand`, `StartAndExpand`)
  - These are deprecated in .NET 9 and will be removed
  - Use regular options: `Fill`, `Start`, `End`, `Center`
  - If you need expansion behavior, use `Grid` with `Height="*"` or `Width="*"` row/column definitions

### Grid Layouts

- **CRITICAL**: Always define explicit `RowDefinitions` and `ColumnDefinitions` in Grid
  - .NET MAUI does NOT auto-generate rows/columns like Xamarin.Forms did
  - Example:

```
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
    </Grid.RowDefinitions>
    <Label Grid.Row="0" Text="Header" />
    <ContentView Grid.Row="1" />
</Grid>
```

### Default Spacing

- Add implicit styles for default spacing (MAUI defaults to 0, unlike Xamarin.Forms which used 6):

```

<Style TargetType="Grid">
    <Setter Property="ColumnSpacing" Value="6" />
    <Setter Property="RowSpacing" Value="6" />
</Style>
<Style TargetType="StackLayout">
    <Setter Property="Spacing" Value="6" />
</Style>
```

### XAML Namespaces

- Use the standard .NET MAUI namespace: `xmlns="http://schemas.microsoft.com/dotnet/2021/maui"`
- Use the standard XAML namespace: `xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"`

### Application Lifecycle

- **CRITICAL**: Use `CreateWindow()` pattern, NOT `MainPage` property (obsolete in .NET 9):

```

public App()
{
    InitializeComponent();
}

protected override Window CreateWindow(IActivationState? activationState)
{
    return new Window(new AppShell());
}

```

### Platform-Specific APIs

- Use `DeviceInfo.Platform` instead of `Device.RuntimePlatform` (Device class is deprecated)
- Use `DeviceInfo.Idiom` instead of `Device.Idiom`
- Use `MainThread.InvokeOnMainThreadAsync()` instead of `Device.InvokeOnMainThreadAsync()`
- Platform constants: `DevicePlatform.Android`, `DevicePlatform.iOS`, `DevicePlatform.MacCatalyst`, `DevicePlatform.WinUI`

### Accessibility & Semantics

- Use `SemanticProperties.Description` instead of `AutomationProperties.Name` (deprecated in .NET 8)
- Use `SemanticProperties.Hint` instead of `AutomationProperties.HelpText` (deprecated in .NET 8)

### Messaging

- Use `WeakReferenceMessenger` from `CommunityToolkit.Mvvm` instead of `MessagingCenter` (deprecated in .NET 9)
- Define message classes and implement `IRecipient<TMessage>` for type-safe messaging

## MVVM with .NET Community Toolkit

This project uses C# and XAML with an MVVM architecture using the .NET Community Toolkit.

### Commands

Use `RelayCommand` for commands that do not return a value.

```

[RelayCommand]
Task DoSomethingAsync()
{
    // Your code here
}

```

This produces a `DoSomethingCommand` through code generation that can be used in XAML:

```

<Button Command="{Binding DoSomethingCommand}" Text="Do Something" />
```

### Observable Properties

Use `ObservableProperty` for data-bindable properties:

```

[ObservableProperty]
private string title = "Default Title";

```

This generates a public `Title` property with `INotifyPropertyChanged` support.

### Dependency Injection

Register services in `MauiProgram.cs`:

```

builder.Services.AddSingleton<IMyService, MyService>();
builder.Services.AddTransient<MyViewModel>();
builder.Services.AddTransient<MyPage>();

```

Inject dependencies via constructor:

```

public class MyViewModel
{
private readonly IMyService _service;

    public MyViewModel(IMyService service)
    {
        _service = service;
    }
}

```

## Code Quality Guidelines

### Global Usings

- Keep `GlobalUsings.cs` minimal - only commonly used namespaces
- Common global usings for MAUI:

```

global using Microsoft.Maui.Controls;
global using Microsoft.Maui.Controls.Xaml;
global using System.Collections.ObjectModel;
global using System.ComponentModel;
global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Mvvm.Input;

```

### Resource Naming

- **CRITICAL**: Image files must follow strict naming rules:
  - Lowercase only
  - Start and end with a letter
  - Only alphanumeric characters or underscores
  - No spaces, hyphens, or special characters
  - Examples: `dotnetbot.png`, `app_icon.svg`, `background_image.jpg`

### Layout Collection Management

Use direct `Add()` methods instead of manipulating `Children` collection:

```
// CORRECT
grid.Add(new Label { Text = "Hello" });

// WRONG - deprecated pattern
grid.Children.Add(new Label { Text = "Hello" });
```