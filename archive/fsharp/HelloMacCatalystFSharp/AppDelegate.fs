namespace HelloMacCatalystFSharp

open System

open UIKit
open Foundation

[<Register ("AppDelegate")>]
type AppDelegate () =
    inherit UIApplicationDelegate ()

    override val Window = null with get, set

    // This method is invoked when the application is ready to run.
    override this.FinishedLaunching (app, options) =
        this.Window <- new UIWindow(UIScreen.MainScreen.Bounds)
        let mutable vc = new UIViewController()
        let label = new UILabel(this.Window.Frame)
        label.TextAlignment <- UITextAlignment.Center
        label.Text <- "Hello, .NET 6!"
        label.TextColor <- UIColor.Blue
        vc.View.AddSubview(label)
        this.Window.RootViewController <- vc
        // make the window visible
        this.Window.MakeKeyAndVisible()
        true