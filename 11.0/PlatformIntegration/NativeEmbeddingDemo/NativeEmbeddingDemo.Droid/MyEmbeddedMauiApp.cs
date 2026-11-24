namespace NativeEmbeddingDemo.Droid;

public static class MyEmbeddedMauiApp
{
    static MauiApp? _shared;

    public static MauiApp Shared =>
        _shared ??= MauiProgram.CreateMauiApp();
}
