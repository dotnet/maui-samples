Use these general guidelines when doing anything for .NET MAUI.

## Page Lifecycle

Use `EventToCommandBehavior` from CommunityToolkit.Maui to handle page lifecycle events when using XAML.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:pageModels="clr-namespace:CommunityTemplate.PageModels"             
             xmlns:models="clr-namespace:CommunityTemplate.Models"
             xmlns:controls="clr-namespace:CommunityTemplate.Pages.Controls"
             xmlns:pullToRefresh="clr-namespace:Syncfusion.Maui.Toolkit.PullToRefresh;assembly=Syncfusion.Maui.Toolkit"
             xmlns:toolkit="http://schemas.microsoft.com/dotnet/2022/maui/toolkit"
             x:Class="CommunityTemplate.Pages.MainPage"
             x:DataType="pageModels:MainPageModel"
             Title="{Binding Today}">

    <ContentPage.Behaviors>
        <toolkit:EventToCommandBehavior
                EventName="NavigatedTo"
                Command="{Binding NavigatedToCommand}" />
        <toolkit:EventToCommandBehavior
                EventName="NavigatedFrom"
                Command="{Binding NavigatedFromCommand}" />
        <toolkit:EventToCommandBehavior
                EventName="Appearing"                
                Command="{Binding AppearingCommand}" />
    </ContentPage.Behaviors>
```

## Control Choices

* Prefer `Grid` over other layouts to keep the visual tree flatter
* Use `VerticalStackLayout` or `HorizontalStackLayout`, not `StackLayout`
* Use `CollectionView` or a `BindableLayout`, not `ListView` or `TableView`
* Use `BindableLayout` when the items source is expected to not exceed 20 items, otherwise use a `CollectionView`
* Use `Border`, not `Frame`
* Declare `ColumnDefinitions` and `RowDefinitions` inline like `<Grid RowDefinitions="*,*,40">`

## Custom Handlers

* Handler registration should be done in MauiProgram.cs within the builder.ConfigureHandlers method

```csharp
builder.ConfigureMauiHandlers(handlers =>
{
        handlers.AddHandler(typeof(Button), typeof(ButtonHandler));
}
```

## UI Components and Controls

- **Good Practice**: Ensure that any UI components or controls are compatible with .NET MAUI.
- **Bad Practice**: Using Xamarin.Forms-specific code unless there is a direct .NET MAUI equivalent.

```csharp
// Good Practice
public class CustomButton : Button
{
    // .NET MAUI specific implementation
}

// Bad Practice
public class CustomButton : Xamarin.Forms.Button
{
    // Xamarin.Forms specific implementation
}
```

## Handling Permissions

- **Good Practice**: Use `Permissions` API to request and check permissions.
- **Bad Practice**: Not handling permissions or using platform-specific code.

```csharp
// Good Practice
public async Task<PermissionStatus> CheckAndRequestLocationPermissionAsync()
{
    var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
    if (status != PermissionStatus.Granted)
    {
        status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
    }
    return status;
}

// Bad Practice
public async Task<bool> CheckAndRequestLocationPermissionAsync()
{
#if ANDROID
    var status = ContextCompat.CheckSelfPermission(Android.App.Application.Context, Manifest.Permission.AccessFineLocation);
    if (status != Permission.Granted)
    {
        ActivityCompat.RequestPermissions(MainActivity.Instance, new[] { Manifest.Permission.AccessFineLocation }, 0);
    }
    return status == Permission.Granted;
#elif IOS
    var status = CLLocationManager.Status;
    if (status != CLAuthorizationStatus.AuthorizedWhenInUse)
    {
        locationManager.RequestWhenInUseAuthorization();
    }
    return status == CLAuthorizationStatus.AuthorizedWhenInUse;
#endif
}
```

## Using Dependency Injection

- **Good Practice**: Use dependency injection to manage dependencies and improve testability.
- **Bad Practice**: Creating instances of dependencies directly within the class.

```csharp
// Good Practice
public class LocationService
{
    private readonly IGeolocation _geolocation;

    public LocationService(IGeolocation geolocation)
    {
        _geolocation = geolocation ?? throw new ArgumentNullException(nameof(geolocation));
    }

    public async Task<Location> GetLocationAsync(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        return await _geolocation.GetLocationAsync(new GeolocationRequest(), token);
    }
}

// Bad Practice
public class LocationService
{
    private readonly IGeolocation _geolocation = new Geolocation();

    public async Task<Location> GetLocationAsync(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        return await _geolocation.GetLocationAsync(new GeolocationRequest(), token);
    }
}
```

## Image Source Initialization

- **Good Practice**: Use dependency injection to initialize image sources.
- **Bad Practice**: Creating instances of image sources directly within the class.

```csharp
// Good Practice
public class CustomImageSource
{
    private readonly IImageService _imageService;

    public CustomImageSource(IImageService imageService)
    {
        _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
    }

    public async Task<ImageSource> GetImageSourceAsync(string imageUrl, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        return await _imageService.LoadImageAsync(imageUrl, token);
    }
}

// Bad Practice
public class CustomImageSource
{
    private readonly IImageService _imageService = new ImageService();

    public async Task<ImageSource> GetImageSourceAsync(string imageUrl)
    {
        return await _imageService.LoadImageAsync(imageUrl);
    }
}
```

## Handling Image Loading

- **Good Practice**: Implement proper error handling and logging when loading images.
- **Bad Practice**: Not handling exceptions or logging errors.

```csharp
// Good Practice
public async Task<ImageSource> GetImageSourceAsync(string imageUrl, CancellationToken token = default)
{
    try
    {
        token.ThrowIfCancellationRequested();
        return await _imageService.LoadImageAsync(imageUrl, token);
    }
    catch (Exception ex)
    {
        Trace.WriteLine($"Error loading image: {ex.Message}");
        return null;
    }
}

// Bad Practice
public async Task<ImageSource> GetImageSourceAsync(string imageUrl)
{
    return await _imageService.LoadImageAsync(imageUrl);
}
```

## Caching Image Sources

- **Good Practice**: Use caching mechanisms to improve performance and reduce network usage.
- **Bad Practice**: Not implementing caching for frequently used images.

```csharp
// Good Practice
public class CachedImageSource
{
    private readonly IImageService _imageService;
    private readonly IMemoryCache _cache;

    public CachedImageSource(IImageService imageService, IMemoryCache cache)
    {
        _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<ImageSource> GetImageSourceAsync(string imageUrl, CancellationToken token = default)
    {
        if (_cache.TryGetValue(imageUrl, out ImageSource cachedImage))
        {
            return cachedImage;
        }

        token.ThrowIfCancellationRequested();
        var imageSource = await _imageService.LoadImageAsync(imageUrl, token);
        _cache.Set(imageUrl, imageSource);
        return imageSource;
    }
}

// Bad Practice
public class CachedImageSource
{
    private readonly IImageService _imageService;

    public CachedImageSource(IImageService imageService)
    {
        _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
    }

    public async Task<ImageSource> GetImageSourceAsync(string imageUrl)
    {
        return await _imageService.LoadImageAsync(imageUrl);
    }
}
```

# Additional prompts

Read these additional prompts when you doing work related to the link's name:

- [layout](maui-layouts.prompt.md)
- [memory leaks](maui-memory-leaks.prompt.md)
- [upgrade to .net maui](maui-upgrade.prompt.md)