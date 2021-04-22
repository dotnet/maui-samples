namespace HelloMacOSFSharp
open System
open AppKit
open Foundation

[<Register ("ViewController")>]
type ViewController(handle: IntPtr) = 
    inherit NSViewController(handle)
    