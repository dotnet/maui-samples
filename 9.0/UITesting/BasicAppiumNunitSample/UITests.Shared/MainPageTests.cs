using NUnit.Framework;

// You will have to make sure that all the namespaces match
// between the different platform specific projects and the shared
// code files. This has to do with how we initialize the AppiumDriver
// through the AppiumSetup.cs files and NUnit SetUpFixture attributes.
// Also see: https://docs.nunit.org/articles/nunit/writing-tests/attributes/setupfixture.html
namespace UITests;

// This is an example of tests that do not need anything platform specific
public class MainPageTests : BaseTest
{
	[Test]
	public void AppLaunches()
	{
		App.GetScreenshot().SaveAsFile($"{nameof(AppLaunches)}.png");
	}

	[Test]
	public void ClickCounterTest()
	{
		// Arrange
		// Find elements with the value of the AutomationId property
		var element = FindUIElement("CounterBtn");

		// Act
		element.Click();
		Task.Delay(500).Wait(); // Wait for the click to register and show up on the screenshot

		// Assert
		App.GetScreenshot().SaveAsFile($"{nameof(ClickCounterTest)}.png");
		Assert.That(element.Text, Is.EqualTo("Clicked 1 time"));
	}
}