namespace SamplePackage;

public static class AppBuilderExtensions
{
    public static MauiAppBuilder UseSamplePackage(this MauiAppBuilder builder)
    {
        builder.ConfigureFonts(fonts =>
        {
            fonts.AddFont("Font Awesome 6 Free-Regular-400.otf", "FontAwesome");
        });

        return builder;
    }
}