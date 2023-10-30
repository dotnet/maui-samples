---
name: .NET MAUI - UI Testing with Appium and NUnit
description: Sample solution that demonstrates how to setup your .NET MAUI app for UI testing with Appium and NUnit.
page_type: sample
languages:
- csharp
- xaml
products:
- dotnet-maui
urlFragment: uitest-appium
---

# UI Testing with Appium and .NET MAUI

This project serves as a bare bones project structure that can be used as an example on how to get started with UI tests using Appium and .NET Multi-platform App UI (MAUI).

As a test framework [NUnit](https://nunit.org/) has been chosen for this example. The tests are ran through Appium. In this code we have chosen to use the release candidate of the Appium v5 NuGet. During our development this has proven to be very stable and with this we were able to make our code significantly cleaner.

## Project Structure

Underneath you will find an overview of all the projects found in this solution and a brief description of what they contain and how they fit together.

* BasicAppiumSample: a File > New .NET MAUI project, meaning, a .NET MAUI project as you would start a new one from Visual Studio or the command-line. The only thing that was added is the `Button` in the `MainPage.xaml` got an addition attribute: `AutomationId`. This is the identifier used to locate the button in our UI tests.

* UITests.Shared: contains code that is shared across all the platforms as well as tests that will run on all the platforms targeted. For example, this project contains the `MainPageTests.cs`. The tests in this class are platform agnostic and will run for Android, iOS, macOS and Windows.

  This shared project also includes the `AppiumServerHelper.cs` which contains code to start and stop the Appium server as part of running the tests.

* UITests.Android: contains code to bootstrap the UI tests for running on Android as well as Android-specific tests.
* UITests.iOS: contains code to bootstrap the UI tests for running on iOS as well as iOS-specific tests.
* UITests.macOS: contains code to bootstrap the UI tests for running on macOS as well as macOS-specific tests.
* UITests.Windows: contains code to bootstrap the UI tests for running on Windows as well as Windows-specific tests.

If you do not need a certain platform, you can safely remove oneor more of the platform-specific projects according to your needs.

Each of the platform-specific projects contains a `AppiumSetup.cs` file. This file contains the details for said platform and configures the values accordingly. Each class contains inline comments to describe what they are.

The most important thing is to change the unique app identifier to your own. This can be either the path to the app executable or the app unique identifier  (for example: com.mycompany.myapp).

### Code Share Considerations

While .NET MAUI by default uses the single-project approach, this is not necesserily desired for UI testing. Therefore, there is a project for each platform you want to target. However, the tests run best if they are bundled into 1 assembly.

To make this possible we are using a so-called [NoTargets project](https://github.com/microsoft/MSBuildSdks/blob/main/src/NoTargets/). This type of project produces no assembly of their own. Instead, it acts as a collection of files that are easily accessible from within Visual Studio, including adding, removing and editing capabilities.

In each of the platform-specific projects, we then have (invisible) links to the files of this project and compile those together with the platform-specific tests. There is no project reference between the shared project and the platform projects. The link is created through the csproj file of each of the platform projects.

This way, all the code ends up in 1 assembly making it easier to run the UI tests.

Typically, you should not notice any of this or have to worry about it.

## Getting Started

Tests are executed through [Appium](https://appium.io/). Appium works by having a server execute the tests on a client application that runs on the target device. This means that running the tests will require an Appium server to run. In this example, code is provided to automatically start and stop the Appium server as part of the test run. However, you will still need to install Appium and prerequisites on the machine that you want to run the tests on.

These prerequisites are:

* [Node.js](https://nodejs.org/): needed to run Appium
* Appium version 2.0+
* Appium Drivers
  * UIAutomator2 for testing your Android app (can be used on both macOS and Windows)
  * XCUITest for testing your iOS app (only available on macOS)
  * Mac2 for testing your macOS app (only available on macOS)
    * For macOS also [Carthage](https://github.com/Carthage/Carthage) is needed
  * Windows for testing your Windows app (only available on Windows)
* For Windows install [WinAppDriver](https://github.com/microsoft/WinAppDriver) make sure to use version 1.2.1. Other versions might not work.

<!-- TODO: Link to instructions -->

## Writing Tests

Ideally the majority of your tests will be under the `UITests.Shared` project. The goal of .NET MAUI is to write your code once and deploy to all the different platforms. As a result, the tests should also be able to run from a single codebase and still test your app on all targeted platforms.

However, there might still be scenarios where you have the need for platform specific tests. Those can be placed under each respective platform project.

The NUnit framework is used in this solution, so writing tests will use all the features that NUnit has to offer us. Let's have a look at sample tests that are included in this project, the shown file is the `MainPageTests.cs` under `UITests.Shared`.

```csharp
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
```

From top to bottom. First, notice how this class inherits from `BaseTest`. This base class contains the `App` field, which you can use to interact with your app. If you peek a bit further on in the code, you can see how this is used to do `GetScreenshot()` to capture a screenshot of your app while running the test. This is (purposely) alike how Xamarin.UITest interacts with the app under test.

The `App` can also find user interface elements. You can use multiple ways to identify an element, but where you can, make sure to add the `AutomationId` property to your .NET MAUI element and identify your element through that in your test. The `BaseTest` also includes a `FindUIElement` helper method. Because Windows uses a different way of identifying UI elements, this helper method is there to unify the API and ensure our tests run without any issue on all platforms. Under the hood this code executes: `App.FindElement(MobileBy.Id(id));` where `id` is the value for `AutomationId` in your .NET MAUI app.

Each method is adorned with a `[Test]` attribute. This marks that method as a test that can be discovered and is ran. Then inside of those methods you can write your tests as you please. Use the `App` object to interact with your .NET MAUI app and use the NUnit assert statements to verify the outcome.

## Running Tests Locally

Tests can be ran from the Visual Studio Test Explorer or by simply running `dotnet test` from the command-line.

To run the tests, an Appium server needs to be available and running, and that in turn should be able to reach the emulators, Simulator or physical devices as needed.

> ![NOTE]
> For all platforms apart from macOS, you typically want to have your app deployed on the device that you want to test on. Make sure you have the latest version on there. Tests will be ran against that app. The way this sample is set up, it will **not** deploy the app for you as part of the test run.

This sample does automatically start and stops the Appium server for you as part of the test run. This is assuming that all the prerequisites are installed properly.

If you want to start the Appium server manually, go into each `AppiumSetup.cs` file in each of the platform projects and comment out the lines that call on `AppiumServerHelper`, there are two, one to start the server and one to stop it.

You will have to make sure the Appium server is started before running the tests and optionally configure the Appium drivers used in code to be able to reach your own server.
