namespace TestHarnessApp;

public partial class TestApp : NativeEmbeddingDemo.App
{
	public TestApp()
	{
		var baseResources = Resources;

		InitializeComponent();

		Resources.MergedDictionaries.Add(baseResources);

		MainPage = new HostPage();
	}
}
