namespace HelloiOSFSharp

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
        let backgroundColor = if UIDevice.CurrentDevice.CheckSystemVersion(13, 0) then UIColor.SystemBackgroundColor else UIColor.White
        let label = new UILabel(this.Window.Frame)
        label.BackgroundColor <- backgroundColor
        label.TextAlignment <- UITextAlignment.Center
        label.Text <- "Hello, .NET 6!"
        label.TextColor <- UIColor.Blue
        vc.View.AddSubview(label)
        this.Window.RootViewController <- vc
        this.Window.MakeKeyAndVisible()
        true

