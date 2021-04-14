namespace HelloMacOSFSharp
open AppKit
module Main = 
    [<EntryPoint>]
    let main args = 
        NSApplication.Init();
        NSApplication.Main(args)
        0