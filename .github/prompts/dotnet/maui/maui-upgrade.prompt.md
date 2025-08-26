Here are important things to know when upgrading code from Xamarin.Forms to .NET MAUI.

Changes to the projects *.csproj files:

- Convert any Xamarin.Forms class library project, Xamarin.iOS project, and Xamarin.Android project to SDK-style projects.
- Update the target framework in project files to net9.0-android and net9.0-ios, as required.
- Set <UseMaui>true</UseMaui> in project files.
- Remove NuGet package references incompatible with .NET 9 alternatives
- Add additional project properties, and remove project properties that aren't required.
- Replace the Xamarin.CommunityToolkit NuGet package with the .NET MAUI Community Toolkit NuGet package.
- Replace Xamarin.Forms compatible versions of the SkiaSharp NuGet packages with .NET MAUI compatible versions, if used.
- Remove references to the Xamarin.Essentials namespace, and replace the Xamarin.Forms namespace with the Microsoft.Maui and Microsoft.Maui.Controls namespaces.

## Layout Changes

- Review all Grid layouts and add explicit RowDefinitions and ColumnDefinitions to replace the auto-generated ones from Xamarin.Forms.
- Search for *AndExpand usage in StackLayout, HorizontalStackLayout, and VerticalStackLayout; remove or refactor them as they are treated as regular Fill options in .NET MAUI.
- Identify and update any RelativeLayout implementations. Refactor the layout to use Grid instead.
- Add implicit styles in the resource dictionaries to set default spacing (e.g., Grid.ColumnSpacing, Grid.RowSpacing, StackLayout.Spacing) to 6 unless a implicit styles already exist.
- Ensure that explicit sizing is applied to controls, as .NET MAUI enforces device-independent units exactly as specified.
- Review ScrollView usage in infinite layouts like VerticalStackLayout; constrain its size to enable proper scrolling behavior.
- Check usages of Frame and update them to use Border.