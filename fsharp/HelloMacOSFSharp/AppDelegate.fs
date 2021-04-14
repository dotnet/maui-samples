namespace HelloMacOSFSharp
open AppKit
open Foundation

[<Register "AppDelegate">]
type AppDelegate () =
    inherit NSApplicationDelegate ()
