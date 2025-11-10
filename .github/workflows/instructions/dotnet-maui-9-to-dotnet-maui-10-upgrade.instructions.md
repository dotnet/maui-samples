# Upgrading from .NET MAUI 9 to .NET MAUI 10

This guide helps you upgrade your .NET MAUI application from .NET 9 to .NET 10 by focusing on the critical breaking changes and obsolete APIs that require code updates.

---

## Table of Contents

1. [Quick Start](#quick-start)
2. [Update Target Framework](#update-target-framework)
3. [Breaking Changes (P0 - Must Fix)](#breaking-changes-p0---must-fix)
4. [Deprecated APIs (P1 - Fix Soon)](#deprecated-apis-p1---fix-soon)
5. [Recommended Changes (P2)](#recommended-changes-p2)
6. [Bulk Migration Tools](#bulk-migration-tools)
7. [Testing Your Upgrade](#testing-your-upgrade)
8. [Troubleshooting](#troubleshooting)

---

## Quick Start

**Three-Step Upgrade Process:**

1. **Update TargetFramework** to `net10.0`
2. **Fix breaking changes** - MessagingCenter (P0)
3. **Fix deprecated APIs** - Animation methods, DisplayAlert, IsBusy (P1)

**Estimated Time:** 1-3 hours depending on codebase size

---

## Update Target Framework

### Single Platform

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
</Project>
```

### Multi-Platform

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net10.0-android;net10.0-ios;net10.0-maccatalyst;net10.0-windows10.0.22621.0</TargetFrameworks>
  </PropertyGroup>
</Project>
```

---

## Breaking Changes (P0 - Must Fix)

### MessagingCenter Made Internal

**Status:** 🚨 **BREAKING** - `MessagingCenter` is now `internal` and cannot be accessed.

**Error You'll See:**
```
error CS0122: 'MessagingCenter' is inaccessible due to its protection level
```

**Migration Required:**

#### Step 1: Install CommunityToolkit.Mvvm

```bash
dotnet add package CommunityToolkit.Mvvm --version 8.3.0
```

#### Step 2: Define Message Classes

```csharp
// OLD: No message class needed
MessagingCenter.Send(this, "UserLoggedIn", userData);

// NEW: Create a message class
public class UserLoggedInMessage
{
    public UserData Data { get; set; }
    
    public UserLoggedInMessage(UserData data)
    {
        Data = data;
    }
}
```

#### Step 3: Update Send Calls

```csharp
// ❌ OLD (Broken in .NET 10)
using Microsoft.Maui.Controls;

MessagingCenter.Send(this, "UserLoggedIn", userData);
MessagingCenter.Send<App, string>(this, "StatusChanged", "Active");

// ✅ NEW (Required)
using CommunityToolkit.Mvvm.Messaging;

WeakReferenceMessenger.Default.Send(new UserLoggedInMessage(userData));
WeakReferenceMessenger.Default.Send(new StatusChangedMessage("Active"));
```

#### Step 4: Update Subscribe Calls

```csharp
// ❌ OLD (Broken in .NET 10)
MessagingCenter.Subscribe<App, UserData>(this, "UserLoggedIn", (sender, data) =>
{
    // Handle message
    CurrentUser = data;
});

// ✅ NEW (Required)
WeakReferenceMessenger.Default.Register<UserLoggedInMessage>(this, (recipient, message) =>
{
    // Handle message
    CurrentUser = message.Data;
});
```

#### Step 5: Unregister When Done

```csharp
// ❌ OLD
MessagingCenter.Unsubscribe<App, UserData>(this, "UserLoggedIn");

// ✅ NEW (CRITICAL - prevents memory leaks)
WeakReferenceMessenger.Default.Unregister<UserLoggedInMessage>(this);

// Or unregister all messages for this recipient
WeakReferenceMessenger.Default.UnregisterAll(this);
```

#### Complete Before/After Example

**Before (.NET 9):**
```csharp
// Sender
public class LoginViewModel
{
    public async Task LoginAsync()
    {
        var user = await AuthService.LoginAsync(username, password);
        MessagingCenter.Send(this, "UserLoggedIn", user);
    }
}

// Receiver
public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        
        MessagingCenter.Subscribe<LoginViewModel, User>(this, "UserLoggedIn", (sender, user) =>
        {
            WelcomeLabel.Text = $"Welcome, {user.Name}!";
        });
    }
    
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MessagingCenter.Unsubscribe<LoginViewModel, User>(this, "UserLoggedIn");
    }
}
```

**After (.NET 10):**
```csharp
// 1. Define message
public class UserLoggedInMessage
{
    public User User { get; }
    
    public UserLoggedInMessage(User user)
    {
        User = user;
    }
}

// 2. Sender
public class LoginViewModel
{
    public async Task LoginAsync()
    {
        var user = await AuthService.LoginAsync(username, password);
        WeakReferenceMessenger.Default.Send(new UserLoggedInMessage(user));
    }
}

// 3. Receiver
public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        
        WeakReferenceMessenger.Default.Register<UserLoggedInMessage>(this, (recipient, message) =>
        {
            WelcomeLabel.Text = $"Welcome, {message.User.Name}!";
        });
    }
    
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }
}
```

**Key Differences:**
- ✅ Type-safe message classes
- ✅ No magic strings
- ✅ Better IntelliSense support
- ✅ Easier to refactor
- ⚠️ **Must remember to unregister!**

---

## Deprecated APIs (P1 - Fix Soon)

These APIs still work in .NET 10 but show compiler warnings. They will be removed in future versions.

### 1. Animation Methods

**Status:** ⚠️ **DEPRECATED** - All sync animation methods replaced with async versions.

**Warning You'll See:**
```
warning CS0618: 'ViewExtensions.FadeTo(VisualElement, double, uint, Easing)' is obsolete: 'Please use FadeToAsync instead.'
```

**Migration Table:**

| Old Method | New Method | Example |
|-----------|-----------|---------|
| `FadeTo()` | `FadeToAsync()` | `await view.FadeToAsync(0, 500);` |
| `ScaleTo()` | `ScaleToAsync()` | `await view.ScaleToAsync(1.5, 300);` |
| `TranslateTo()` | `TranslateToAsync()` | `await view.TranslateToAsync(100, 100, 250);` |
| `RotateTo()` | `RotateToAsync()` | `await view.RotateToAsync(360, 500);` |
| `RotateXTo()` | `RotateXToAsync()` | `await view.RotateXToAsync(45, 300);` |
| `RotateYTo()` | `RotateYToAsync()` | `await view.RotateYToAsync(45, 300);` |
| `ScaleXTo()` | `ScaleXToAsync()` | `await view.ScaleXToAsync(2.0, 300);` |
| `ScaleYTo()` | `ScaleYToAsync()` | `await view.ScaleYToAsync(2.0, 300);` |
| `RelRotateTo()` | `RelRotateToAsync()` | `await view.RelRotateToAsync(90, 300);` |
| `RelScaleTo()` | `RelScaleToAsync()` | `await view.RelScaleToAsync(0.5, 300);` |
| `LayoutTo()` | `LayoutToAsync()` | See special note below |

#### Migration Examples

**Simple Animation:**
```csharp
// ❌ OLD (Deprecated)
await myButton.FadeTo(0, 500);
await myButton.ScaleTo(1.5, 300);
await myButton.TranslateTo(100, 100, 250);

// ✅ NEW (Required)
await myButton.FadeToAsync(0, 500);
await myButton.ScaleToAsync(1.5, 300);
await myButton.TranslateToAsync(100, 100, 250);
```

**Sequential Animations:**
```csharp
// ❌ OLD
await image.FadeTo(0, 300);
await image.ScaleTo(0.5, 300);
await image.FadeTo(1, 300);

// ✅ NEW
await image.FadeToAsync(0, 300);
await image.ScaleToAsync(0.5, 300);
await image.FadeToAsync(1, 300);
```

**Parallel Animations:**
```csharp
// ❌ OLD
await Task.WhenAll(
    image.FadeTo(0, 300),
    image.ScaleTo(0.5, 300),
    image.RotateTo(360, 300)
);

// ✅ NEW
await Task.WhenAll(
    image.FadeToAsync(0, 300),
    image.ScaleToAsync(0.5, 300),
    image.RotateToAsync(360, 300)
);
```

**With Cancellation:**
```csharp
// NEW: Async methods support cancellation
CancellationTokenSource cts = new();

try
{
    await view.FadeToAsync(0, 2000);
}
catch (TaskCanceledException)
{
    // Animation was cancelled
}

// Cancel from elsewhere
cts.Cancel();
```

#### Special Case: LayoutTo

`LayoutToAsync()` is deprecated with a special message: "Use Translation to animate layout changes."

```csharp
// ❌ OLD (Deprecated)
await view.LayoutToAsync(new Rect(100, 100, 200, 200), 250);

// ✅ NEW (Use TranslateToAsync instead)
await view.TranslateToAsync(100, 100, 250);

// Or animate Translation properties directly
var animation = new Animation(v => view.TranslationX = v, 0, 100);
animation.Commit(view, "MoveX", length: 250);
```

---

### 2. DisplayAlert and DisplayActionSheet

**Status:** ⚠️ **DEPRECATED** - Sync methods replaced with async versions.

**Warning You'll See:**
```
warning CS0618: 'Page.DisplayAlert(string, string, string)' is obsolete: 'Use DisplayAlertAsync instead'
```

#### Migration Examples

**DisplayAlert:**
```csharp
// ❌ OLD (Deprecated)
await DisplayAlert("Success", "Data saved successfully", "OK");
await DisplayAlert("Error", "Failed to save", "Cancel");
bool result = await DisplayAlert("Confirm", "Delete this item?", "Yes", "No");

// ✅ NEW (Required)
await DisplayAlertAsync("Success", "Data saved successfully", "OK");
await DisplayAlertAsync("Error", "Failed to save", "Cancel");
bool result = await DisplayAlertAsync("Confirm", "Delete this item?", "Yes", "No");
```

**DisplayActionSheet:**
```csharp
// ❌ OLD (Deprecated)
string action = await DisplayActionSheet(
    "Choose an action",
    "Cancel",
    "Delete",
    "Edit", "Share", "Duplicate"
);

// ✅ NEW (Required)
string action = await DisplayActionSheetAsync(
    "Choose an action",
    "Cancel",
    "Delete",
    "Edit", "Share", "Duplicate"
);
```

**In ViewModels (with IDispatcher):**
```csharp
// If you're calling from a ViewModel, you'll need access to a Page
public class MyViewModel
{
    private readonly IDispatcher _dispatcher;
    private readonly Page _page;
    
    public MyViewModel(IDispatcher dispatcher, Page page)
    {
        _dispatcher = dispatcher;
        _page = page;
    }
    
    public async Task ShowAlertAsync()
    {
        await _dispatcher.DispatchAsync(async () =>
        {
            await _page.DisplayAlertAsync("Info", "Message from ViewModel", "OK");
        });
    }
}
```

---

### 3. Page.IsBusy

**Status:** ⚠️ **DEPRECATED** - Property will be removed in .NET 11.

**Warning You'll See:**
```
warning CS0618: 'Page.IsBusy' is obsolete: 'Page.IsBusy has been deprecated and will be removed in .NET 11'
```

**Why It's Deprecated:**
- Inconsistent behavior across platforms
- Limited customization options
- Doesn't work well with modern MVVM patterns

#### Migration Examples

**Simple Page:**
```xaml
<!-- ❌ OLD (Deprecated) -->
<ContentPage IsBusy="{Binding IsLoading}">
    <StackLayout>
        <Label Text="Content here" />
    </StackLayout>
</ContentPage>

<!-- ✅ NEW (Recommended) -->
<ContentPage>
    <Grid>
        <!-- Main content -->
        <StackLayout>
            <Label Text="Content here" />
        </StackLayout>
        
        <!-- Loading indicator overlay -->
        <ActivityIndicator IsRunning="{Binding IsLoading}"
                          IsVisible="{Binding IsLoading}"
                          Color="{StaticResource Primary}"
                          VerticalOptions="Center"
                          HorizontalOptions="Center" />
    </Grid>
</ContentPage>
```

**With Loading Overlay:**
```xaml
<!-- ✅ Better: Custom loading overlay -->
<ContentPage>
    <Grid>
        <!-- Main content -->
        <ScrollView>
            <VerticalStackLayout Padding="20">
                <Label Text="Your content here" />
            </VerticalStackLayout>
        </ScrollView>
        
        <!-- Loading overlay -->
        <Grid IsVisible="{Binding IsLoading}"
              BackgroundColor="#80000000">
            <VerticalStackLayout VerticalOptions="Center"
                               HorizontalOptions="Center"
                               Spacing="10">
                <ActivityIndicator IsRunning="True"
                                 Color="White" />
                <Label Text="Loading..."
                       TextColor="White" />
            </VerticalStackLayout>
        </Grid>
    </Grid>
</ContentPage>
```

**In Code-Behind:**
```csharp
// ❌ OLD (Deprecated)
public partial class MyPage : ContentPage
{
    async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
            await LoadDataFromServerAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }
}

// ✅ NEW (Recommended)
public partial class MyPage : ContentPage
{
    async Task LoadDataAsync()
    {
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;
        try
        {
            await LoadDataFromServerAsync();
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }
}
```

**In ViewModel:**
```csharp
public class MyViewModel : INotifyPropertyChanged
{
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }
    
    public async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            await LoadDataFromServerAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }
}
```

---

## Recommended Changes (P2)

These changes are recommended but not required immediately. Consider migrating during your next refactoring cycle.

### Application.MainPage

**Status:** ⚠️ **DEPRECATED** - Property will be removed in future version.

**Warning You'll See:**
```
warning CS0618: 'Application.MainPage' is obsolete: 'This property is deprecated. Initialize your application by overriding Application.CreateWindow...'
```

#### Migration Example

```csharp
// ❌ OLD (Deprecated)
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }
    
    // Changing page later
    public void SwitchToLoginPage()
    {
        MainPage = new LoginPage();
    }
}

// ✅ NEW (Recommended)
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }
    
    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
    
    // Changing page later
    public void SwitchToLoginPage()
    {
        if (Windows.Count > 0)
        {
            Windows[0].Page = new LoginPage();
        }
    }
}
```

**Benefits of CreateWindow:**
- Better multi-window support
- More explicit initialization
- Cleaner separation of concerns
- Works better with Shell

---

## Bulk Migration Tools

Use these find/replace patterns to quickly update your codebase.

### Visual Studio / VS Code

**Regex Mode - Find/Replace**

#### Animation Methods

```regex
Find:    \.FadeTo\(
Replace: .FadeToAsync(

Find:    \.ScaleTo\(
Replace: .ScaleToAsync(

Find:    \.TranslateTo\(
Replace: .TranslateToAsync(

Find:    \.RotateTo\(
Replace: .RotateToAsync(

Find:    \.RotateXTo\(
Replace: .RotateXToAsync(

Find:    \.RotateYTo\(
Replace: .RotateYToAsync(

Find:    \.ScaleXTo\(
Replace: .ScaleXToAsync(

Find:    \.ScaleYTo\(
Replace: .ScaleYToAsync(

Find:    \.RelRotateTo\(
Replace: .RelRotateToAsync(

Find:    \.RelScaleTo\(
Replace: .RelScaleToAsync(
```

#### Display Methods

```regex
Find:    DisplayAlert\(
Replace: DisplayAlertAsync(

Find:    DisplayActionSheet\(
Replace: DisplayActionSheetAsync(
```

### PowerShell Script

```powershell
# Replace animation methods in all .cs files
Get-ChildItem -Path . -Recurse -Filter *.cs | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    
    # Animation methods
    $content = $content -replace '\.FadeTo\(', '.FadeToAsync('
    $content = $content -replace '\.ScaleTo\(', '.ScaleToAsync('
    $content = $content -replace '\.TranslateTo\(', '.TranslateToAsync('
    $content = $content -replace '\.RotateTo\(', '.RotateToAsync('
    $content = $content -replace '\.RotateXTo\(', '.RotateXToAsync('
    $content = $content -replace '\.RotateYTo\(', '.RotateYToAsync('
    $content = $content -replace '\.ScaleXTo\(', '.ScaleXToAsync('
    $content = $content -replace '\.ScaleYTo\(', '.ScaleYToAsync('
    $content = $content -replace '\.RelRotateTo\(', '.RelRotateToAsync('
    $content = $content -replace '\.RelScaleTo\(', '.RelScaleToAsync('
    
    # Display methods
    $content = $content -replace 'DisplayAlert\(', 'DisplayAlertAsync('
    $content = $content -replace 'DisplayActionSheet\(', 'DisplayActionSheetAsync('
    
    Set-Content $_.FullName $content
}

Write-Host "✅ Migration complete!"
```

---

## Testing Your Upgrade

### Build Validation

```bash
# Clean solution
dotnet clean

# Restore packages
dotnet restore

# Build for each platform
dotnet build -f net10.0-android -c Release
dotnet build -f net10.0-ios -c Release
dotnet build -f net10.0-maccatalyst -c Release
dotnet build -f net10.0-windows -c Release

# Check for warnings
dotnet build --no-incremental 2>&1 | grep -i "warning CS0618"
```

### Enable Warnings as Errors (Temporary)

```xml
<!-- Add to your .csproj to catch all obsolete API usage -->
<PropertyGroup>
  <WarningsAsErrors>CS0618</WarningsAsErrors>
</PropertyGroup>
```

### Test Checklist

- [ ] App launches successfully on all platforms
- [ ] All animations work correctly
- [ ] Dialogs (alerts/action sheets) display properly
- [ ] Loading indicators work (if you used IsBusy)
- [ ] Inter-component communication works (MessagingCenter replacement)
- [ ] No CS0618 warnings in build output
- [ ] No runtime exceptions related to obsolete APIs

---

## Troubleshooting

### Error: 'MessagingCenter' is inaccessible due to its protection level

**Cause:** MessagingCenter is now internal in .NET 10.

**Solution:**
1. Install `CommunityToolkit.Mvvm` package
2. Replace with `WeakReferenceMessenger` (see [MessagingCenter section](#messagingcenter-made-internal))
3. Create message classes for each message type
4. Don't forget to unregister!

---

### Warning: Animation method is obsolete

**Cause:** Using sync animation methods (`FadeTo`, `ScaleTo`, etc.)

**Quick Fix:**
```bash
# Use PowerShell script from Bulk Migration Tools section
# Or use Find/Replace patterns
```

**Manual Fix:**
Add `Async` to the end of each animation method call:
- `FadeTo` → `FadeToAsync`
- `ScaleTo` → `ScaleToAsync`
- etc.

---

### Page.IsBusy doesn't work anymore

**Cause:** IsBusy still works but is deprecated.

**Solution:** Replace with explicit ActivityIndicator (see [IsBusy section](#3-pageisbusy))

---

### Build fails with "Target framework 'net10.0' not found"

**Cause:** .NET 10 SDK not installed or not latest version.

**Solution:**
```bash
# Check SDK version
dotnet --version  # Should be 10.0.100 or later

# Install .NET 10 SDK from:
# https://dotnet.microsoft.com/download/dotnet/10.0

# Update workloads
dotnet workload update
```

---

### MessagingCenter migration breaks existing code

**Common Issues:**

1. **Forgot to unregister:**
   ```csharp
   // ⚠️ Memory leak if you don't unregister
   protected override void OnDisappearing()
   {
       base.OnDisappearing();
       WeakReferenceMessenger.Default.UnregisterAll(this);
   }
   ```

2. **Wrong message type:**
   ```csharp
   // ❌ Wrong
   WeakReferenceMessenger.Default.Register<UserLoggedIn>(this, handler);
   WeakReferenceMessenger.Default.Send(new UserData());  // Wrong type!
   
   // ✅ Correct
   WeakReferenceMessenger.Default.Register<UserLoggedInMessage>(this, handler);
   WeakReferenceMessenger.Default.Send(new UserLoggedInMessage(userData));
   ```

3. **Recipient parameter confusion:**
   ```csharp
   // The recipient parameter is the object that registered (this)
   WeakReferenceMessenger.Default.Register<MyMessage>(this, (recipient, message) =>
   {
       // recipient == this
       // message == the message that was sent
   });
   ```

---

### Animation doesn't complete after migration

**Cause:** Forgetting `await` keyword.

```csharp
// ❌ Wrong - animation runs but code continues immediately
view.FadeToAsync(0, 500);
DoSomethingElse();

// ✅ Correct - wait for animation to complete
await view.FadeToAsync(0, 500);
DoSomethingElse();
```

---

## Quick Reference Card

### Priority Checklist

**Must Fix (P0):**
- [ ] Replace `MessagingCenter` with `WeakReferenceMessenger`

**Should Fix (P1):**
- [ ] Update animation methods: add `Async` suffix
- [ ] Update `DisplayAlert` → `DisplayAlertAsync`
- [ ] Update `DisplayActionSheet` → `DisplayActionSheetAsync`  
- [ ] Replace `Page.IsBusy` with `ActivityIndicator`

**Nice to Have (P2):**
- [ ] Migrate `Application.MainPage` to `CreateWindow`

### Common Patterns

```csharp
// Animation
await view.FadeToAsync(0, 500);

// Alert
await DisplayAlertAsync("Title", "Message", "OK");

// Messaging
WeakReferenceMessenger.Default.Send(new MyMessage());
WeakReferenceMessenger.Default.Register<MyMessage>(this, (r, m) => { });
WeakReferenceMessenger.Default.UnregisterAll(this);

// Loading
IsLoading = true;
try { await LoadAsync(); }
finally { IsLoading = false; }
```

---

## Additional Resources

- **Official Docs:** https://learn.microsoft.com/dotnet/maui/
- **Migration Guide:** https://learn.microsoft.com/dotnet/maui/migration/
- **GitHub Issues:** https://github.com/dotnet/maui/issues
- **CommunityToolkit.Mvvm:** https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/

---

**Document Version:** 2.0  
**Last Updated:** November 2025  
**Applies To:** .NET MAUI 10.0.100 and later
