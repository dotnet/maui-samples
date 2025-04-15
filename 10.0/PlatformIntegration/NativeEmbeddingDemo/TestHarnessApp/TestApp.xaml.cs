namespace TestHarnessApp;

public partial class TestApp : NativeEmbeddingDemo.App
{
	public TestApp()
	{
		var baseResources = Resources;

		InitializeComponent();

		Resources.MergedDictionaries.Add(baseResources);
	}

    protected override Window CreateWindow(IActivationState activationState)
    {
		return new Window(new HostPage());
    }
}
