namespace TestHarnessApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp() =>
        NativeEmbeddingDemo.MauiProgram.CreateMauiApp<TestApp>(builder =>
        {
            // Add any test harness configuration such as service stubs or mocks.
        });
}
